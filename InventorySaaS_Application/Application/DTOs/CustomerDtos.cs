using System;

namespace InventorySaaS_Application.Application.DTOs
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? TaxNumber { get; set; }
        public decimal CreditLimit { get; set; }
        public string? PaymentTerms { get; set; }
        public string Status { get; set; } = default!;
    }

    public class CreateCustomerDto
    {
        public string CustomerCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? TaxNumber { get; set; }
        public decimal CreditLimit { get; set; }
        public string? PaymentTerms { get; set; }
    }
}