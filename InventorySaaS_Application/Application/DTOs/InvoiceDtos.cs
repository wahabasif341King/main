using System;

namespace InventorySaaS_Application.Application.DTOs
{
    public class InvoiceDto
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = default!;
        public Guid? SalesOrderId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = default!;
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; } = default!;
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Existing Sales Order se invoice generate karne ke liye
    public class CreateInvoiceFromSalesOrderDto
    {
        public Guid SalesOrderId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}