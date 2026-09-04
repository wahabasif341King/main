using System;
using System.Collections.Generic;

namespace InventorySaaS_Application.Domain.Entities
{
    public class PurchaseOrder
    {
        public Guid PurchaseOrderId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public Guid WarehouseId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedDeliveryDate { get; set; }

        // Draft -> Sent -> PartiallyReceived -> Received (ya Cancelled)
        public string Status { get; set; } = "Draft";

        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Notes { get; set; }

        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Supplier Supplier { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }
}