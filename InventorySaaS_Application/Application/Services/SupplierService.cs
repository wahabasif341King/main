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
    public class SupplierService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public SupplierService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            return await _context.Suppliers
                .Where(s => s.TenantId == _currentUser.TenantId && s.Status != "Inactive")
                .Select(s => new SupplierDto
                {
                    SupplierId = s.SupplierId,
                    SupplierCode = s.SupplierCode,
                    Name = s.Name,
                    ContactPerson = s.ContactPerson,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    TaxNumber = s.TaxNumber,
                    PaymentTerms = s.PaymentTerms,
                    Status = s.Status
                })
                .ToListAsync();
        }

        public async Task<SupplierDto?> GetByIdAsync(Guid supplierId)
        {
            var s = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.SupplierId == supplierId && x.TenantId == _currentUser.TenantId);

            if (s == null) return null;

            return new SupplierDto
            {
                SupplierId = s.SupplierId,
                SupplierCode = s.SupplierCode,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address,
                TaxNumber = s.TaxNumber,
                PaymentTerms = s.PaymentTerms,
                Status = s.Status
            };
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
        {
            var codeExists = await _context.Suppliers.AnyAsync(s =>
                s.TenantId == _currentUser.TenantId && s.SupplierCode == dto.SupplierCode);

            if (codeExists)
                throw new InvalidOperationException($"Supplier code '{dto.SupplierCode}' already exists.");

            var supplier = new Supplier
            {
                TenantId = _currentUser.TenantId,
                SupplierCode = dto.SupplierCode,
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                TaxNumber = dto.TaxNumber,
                PaymentTerms = dto.PaymentTerms,
                Status = "Active"
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(supplier.SupplierId) ?? throw new InvalidOperationException("Failed to load created supplier.");
        }

        public async Task<SupplierDto?> UpdateAsync(Guid supplierId, CreateSupplierDto dto)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId && s.TenantId == _currentUser.TenantId);

            if (supplier == null) return null;

            var codeExists = await _context.Suppliers.AnyAsync(s =>
                s.TenantId == _currentUser.TenantId &&
                s.SupplierCode == dto.SupplierCode &&
                s.SupplierId != supplierId);

            if (codeExists)
                throw new InvalidOperationException($"Supplier code '{dto.SupplierCode}' already exists.");

            supplier.SupplierCode = dto.SupplierCode;
            supplier.Name = dto.Name;
            supplier.ContactPerson = dto.ContactPerson;
            supplier.Email = dto.Email;
            supplier.Phone = dto.Phone;
            supplier.Address = dto.Address;
            supplier.TaxNumber = dto.TaxNumber;
            supplier.PaymentTerms = dto.PaymentTerms;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(supplierId);
        }

        public async Task<bool> DeleteAsync(Guid supplierId)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId && s.TenantId == _currentUser.TenantId);

            if (supplier == null) return false;

            supplier.Status = "Inactive";
            await _context.SaveChangesAsync();

            return true;
        }
    }
}