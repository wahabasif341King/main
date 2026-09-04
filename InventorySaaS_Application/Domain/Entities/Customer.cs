using System;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? TaxNumber { get; set; }
        public decimal CreditLimit { get; set; } = 0;
        public string? PaymentTerms { get; set; }
        public string Status { get; set; } = "Active"; // Active / Inactive / Blocked
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}