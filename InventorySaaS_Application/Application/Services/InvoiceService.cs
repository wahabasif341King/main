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
    public class InvoiceService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly AuditLogService _auditLog;

        public InvoiceService(AppDbContext context, ICurrentUserService currentUser, AuditLogService auditLog)
        {
            _context = context;
            _currentUser = currentUser;
            _auditLog = auditLog;
        }

        public async Task<List<InvoiceDto>> GetAllAsync()
        {
            return await _context.Invoices
                .Where(i => i.TenantId == _currentUser.TenantId)
                .Include(i => i.Customer)
                .Include(i => i.SalesOrder)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new InvoiceDto
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceNumber = i.InvoiceNumber,
                    SalesOrderId = i.SalesOrderId,
                    SalesOrderNumber = i.SalesOrder != null ? i.SalesOrder.OrderNumber : null,
                    CustomerId = i.CustomerId,
                    CustomerName = i.Customer.Name,
                    Subtotal = i.Subtotal,
                    DiscountAmount = i.DiscountAmount,
                    TaxAmount = i.TaxAmount,
                    ShippingAmount = i.ShippingAmount,
                    GrandTotal = i.GrandTotal,
                    PaidAmount = i.PaidAmount,
                    RemainingAmount = i.RemainingAmount,
                    Status = i.Status,
                    DueDate = i.DueDate,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<InvoiceDto?> GetByIdAsync(Guid id)
        {
            return (await GetAllAsync()).FirstOrDefault(i => i.InvoiceId == id);
        }

        public async Task<InvoiceDto> CreateFromSalesOrderAsync(CreateInvoiceFromSalesOrderDto dto)
        {
            var salesOrder = await _context.SalesOrders
                .FirstOrDefaultAsync(o => o.SalesOrderId == dto.SalesOrderId && o.TenantId == _currentUser.TenantId);

            if (salesOrder == null)
                throw new InvalidOperationException("Sales order not found.");

            var alreadyInvoiced = await _context.Invoices.AnyAsync(i =>
                i.SalesOrderId == dto.SalesOrderId && i.TenantId == _currentUser.TenantId);
            if (alreadyInvoiced)
                throw new InvalidOperationException("This sales order already has an invoice.");

            var invoice = new Invoice
            {
                TenantId = _currentUser.TenantId,
                InvoiceNumber = await GenerateInvoiceNumberAsync(),
                SalesOrderId = salesOrder.SalesOrderId,
                CustomerId = salesOrder.CustomerId,
                Subtotal = salesOrder.Subtotal,
                DiscountAmount = salesOrder.DiscountAmount,
                TaxAmount = salesOrder.TaxAmount,
                ShippingAmount = salesOrder.ShippingAmount,
                GrandTotal = salesOrder.GrandTotal,
                PaidAmount = 0,
                RemainingAmount = salesOrder.GrandTotal,
                Status = "Unpaid",
                DueDate = dto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync("Created", "Invoice", invoice.InvoiceId, newValue: invoice.InvoiceNumber);

            return await GetByIdAsync(invoice.InvoiceId)
                ?? throw new InvalidOperationException("Failed to load created invoice.");
        }

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var count = await _context.Invoices.CountAsync(i => i.TenantId == _currentUser.TenantId);
            return $"INV-{(count + 1):D5}";
        }
    }
}