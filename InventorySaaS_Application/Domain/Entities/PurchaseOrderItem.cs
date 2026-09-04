using System;

namespace InventorySaaS_Application.Domain.Entities
{
    public class PurchaseOrderItem
    {
        public Guid PurchaseOrderItemId { get; set; } = Guid.NewGuid();
        public Guid PurchaseOrderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? VariantId { get; set; }

        public int QuantityOrdered { get; set; }
        public int QuantityReceived { get; set; } = 0;
        public int QuantityDamaged { get; set; } = 0;
        public int QuantityRejected { get; set; } = 0;

        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}