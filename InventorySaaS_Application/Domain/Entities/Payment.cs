using System;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Payment
    {
        public Guid PaymentId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public Guid? InvoiceId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? SupplierId { get; set; }

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // Cash / Bank / Card / Online / Other
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? ReferenceNumber { get; set; }
        public Guid CreatedByUserId { get; set; }

        public Invoice? Invoice { get; set; }
        public Customer? Customer { get; set; }
        public Supplier? Supplier { get; set; }
    }
}