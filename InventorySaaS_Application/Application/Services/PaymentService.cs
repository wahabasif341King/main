using InventorySaaS_Application.Application.DTOs;
using InventorySaaS_Application.Domain.Entities;
using InventorySaaS_Application.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySaaS_Application.Application.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly AuditLogService _auditLog;

        public PaymentService(AppDbContext context, ICurrentUserService currentUser, AuditLogService auditLog)
        {
            _context = context;
            _currentUser = currentUser;
            _auditLog = auditLog;
        }

        public async Task<List<PaymentDto>> GetAllAsync()
        {
            return await _context.Payments
                .Where(p => p.TenantId == _currentUser.TenantId)
                .Include(p => p.Invoice)
                .Include(p => p.Customer)
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new PaymentDto
                {
                    PaymentId = p.PaymentId,
                    InvoiceId = p.InvoiceId,
                    InvoiceNumber = p.Invoice != null ? p.Invoice.InvoiceNumber : null,
                    CustomerId = p.CustomerId,
                    CustomerName = p.Customer != null ? p.Customer.Name : null,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier != null ? p.Supplier.Name : null,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentDate = p.PaymentDate,
                    ReferenceNumber = p.ReferenceNumber
                })
                .ToListAsync();
        }

        public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
        {
            if (dto.Amount <= 0)
                throw new InvalidOperationException("Amount must be greater than zero.");

            if (dto.InvoiceId == null && dto.SupplierId == null)
                throw new InvalidOperationException("Either an Invoice (customer payment) or a Supplier (supplier payment) is required.");

            var payment = new Payment
            {
                TenantId = _currentUser.TenantId,
                InvoiceId = dto.InvoiceId,
                CustomerId = dto.CustomerId,
                SupplierId = dto.SupplierId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                PaymentDate = DateTime.UtcNow,
                ReferenceNumber = dto.ReferenceNumber,
                CreatedByUserId = _currentUser.UserId
            };

            _context.Payments.Add(payment);

            // Agar customer invoice ke against payment hai, to invoice ka Paid/Remaining/Status update karo
            if (dto.InvoiceId.HasValue)
            {
                var invoice = await _context.Invoices.FirstOrDefaultAsync(i =>
                    i.InvoiceId == dto.InvoiceId.Value && i.TenantId == _currentUser.TenantId);

                if (invoice == null)
                    throw new InvalidOperationException("Invoice not found.");

                invoice.PaidAmount += dto.Amount;
                invoice.RemainingAmount = invoice.GrandTotal - invoice.PaidAmount;

                invoice.Status = invoice.RemainingAmount <= 0 ? "Paid"
                    : invoice.PaidAmount > 0 ? "PartiallyPaid"
                    : "Unpaid";
            }

            await _context.SaveChangesAsync();

            await _auditLog.LogAsync(
                dto.SupplierId.HasValue ? "PaymentMade" : "PaymentReceived",
                "Payment",
                payment.PaymentId,
                newValue: dto.Amount.ToString());

            return (await GetAllAsync()).First(p => p.PaymentId == payment.PaymentId);
        }
    }
}