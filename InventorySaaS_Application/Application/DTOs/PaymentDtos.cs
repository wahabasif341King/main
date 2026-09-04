using System;

namespace InventorySaaS_Application.Application.DTOs
{
    public class PaymentDto
    {
        public Guid PaymentId { get; set; }
        public Guid? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public DateTime PaymentDate { get; set; }
        public string? ReferenceNumber { get; set; }
    }

    public class CreatePaymentDto
    {
        public Guid? InvoiceId { get; set; }     // Customer se payment ho to ye lagega
        public Guid? CustomerId { get; set; }
        public Guid? SupplierId { get; set; }    // Supplier ko payment ho to ye lagega
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string? ReferenceNumber { get; set; }
    }
}