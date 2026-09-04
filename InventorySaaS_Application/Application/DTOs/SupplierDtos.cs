using System;

namespace InventorySaaS_Application.Application.DTOs
{
    public class SupplierDto
    {
        public Guid SupplierId { get; set; }
        public string SupplierCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public string? PaymentTerms { get; set; }
        public string Status { get; set; } = default!;
    }

    public class CreateSupplierDto
    {
        public string SupplierCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public string? PaymentTerms { get; set; }
    }
}