using System;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid? SalesOrderId { get; set; }
        public Guid CustomerId { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public decimal RemainingAmount { get; set; }

        // Unpaid -> PartiallyPaid -> Paid (ya Overdue)
        public string Status { get; set; } = "Unpaid";
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public SalesOrder? SalesOrder { get; set; }
        public Customer Customer { get; set; } = null!;
    }
}