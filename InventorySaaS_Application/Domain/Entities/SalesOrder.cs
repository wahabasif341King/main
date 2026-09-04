using System;
using System.Collections.Generic;

namespace InventorySaaS_Application.Domain.Entities
{
    public class SalesOrder
    {
        public Guid SalesOrderId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid WarehouseId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // Draft -> Confirmed -> Processing -> Packed -> Shipped -> Delivered (ya Cancelled)
        public string Status { get; set; } = "Draft";

        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal GrandTotal { get; set; }

        public Guid CreatedByUserId { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Customer Customer { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
    }
}