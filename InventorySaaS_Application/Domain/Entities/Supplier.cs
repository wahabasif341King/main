using System;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Supplier
    {
        public Guid SupplierId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public string SupplierCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public string? PaymentTerms { get; set; }
        public string Status { get; set; } = "Active"; // Active / Inactive
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}