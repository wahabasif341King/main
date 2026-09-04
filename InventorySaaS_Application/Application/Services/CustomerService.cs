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
    public class CustomerService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CustomerService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            return await _context.Customers
                .Where(c => c.TenantId == _currentUser.TenantId && c.Status != "Inactive")
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    CustomerCode = c.CustomerCode,
                    Name = c.Name,
                    CompanyName = c.CompanyName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Address = c.Address,
                    City = c.City,
                    TaxNumber = c.TaxNumber,
                    CreditLimit = c.CreditLimit,
                    PaymentTerms = c.PaymentTerms,
                    Status = c.Status
                })
                .ToListAsync();
        }

        public async Task<CustomerDto?> GetByIdAsync(Guid customerId)
        {
            var c = await _context.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.TenantId == _currentUser.TenantId);

            if (c == null) return null;

            return new CustomerDto
            {
                CustomerId = c.CustomerId,
                CustomerCode = c.CustomerCode,
                Name = c.Name,
                CompanyName = c.CompanyName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                City = c.City,
                TaxNumber = c.TaxNumber,
                CreditLimit = c.CreditLimit,
                PaymentTerms = c.PaymentTerms,
                Status = c.Status
            };
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            var codeExists = await _context.Customers.AnyAsync(c =>
                c.TenantId == _currentUser.TenantId && c.CustomerCode == dto.CustomerCode);

            if (codeExists)
                throw new InvalidOperationException($"Customer code '{dto.CustomerCode}' already exists.");

            var customer = new Customer
            {
                TenantId = _currentUser.TenantId,
                CustomerCode = dto.CustomerCode,
                Name = dto.Name,
                CompanyName = dto.CompanyName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                TaxNumber = dto.TaxNumber,
                CreditLimit = dto.CreditLimit,
                PaymentTerms = dto.PaymentTerms,
                Status = "Active"
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(customer.CustomerId) ?? throw new InvalidOperationException("Failed to load created customer.");
        }

        public async Task<CustomerDto?> UpdateAsync(Guid customerId, CreateCustomerDto dto)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.TenantId == _currentUser.TenantId);

            if (customer == null) return null;

            var codeExists = await _context.Customers.AnyAsync(c =>
                c.TenantId == _currentUser.TenantId &&
                c.CustomerCode == dto.CustomerCode &&
                c.CustomerId != customerId);

            if (codeExists)
                throw new InvalidOperationException($"Customer code '{dto.CustomerCode}' already exists.");

            customer.CustomerCode = dto.CustomerCode;
            customer.Name = dto.Name;
            customer.CompanyName = dto.CompanyName;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.Address = dto.Address;
            customer.City = dto.City;
            customer.TaxNumber = dto.TaxNumber;
            customer.CreditLimit = dto.CreditLimit;
            customer.PaymentTerms = dto.PaymentTerms;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(customerId);
        }

        public async Task<bool> DeleteAsync(Guid customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.TenantId == _currentUser.TenantId);

            if (customer == null) return false;

            // Soft delete — future Sales Orders/Invoices ke references na tootein
            customer.Status = "Inactive";
            await _context.SaveChangesAsync();

            return true;
        }
    }
}