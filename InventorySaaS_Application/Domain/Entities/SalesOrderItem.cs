using System;

namespace InventorySaaS_Application.Domain.Entities
{
    public class SalesOrderItem
    {
        public Guid SalesOrderItemId { get; set; } = Guid.NewGuid();
        public Guid SalesOrderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public SalesOrder SalesOrder { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}