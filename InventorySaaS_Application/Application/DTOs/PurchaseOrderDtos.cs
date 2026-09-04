using System;
using System.Collections.Generic;

namespace InventorySaaS_Application.Application.DTOs
{
    public class PurchaseOrderItemDto
    {
        public Guid PurchaseOrderItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public int QuantityOrdered { get; set; }
        public int QuantityReceived { get; set; }
        public int QuantityDamaged { get; set; }
        public int QuantityRejected { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
    }

    public class PurchaseOrderDto
    {
        public Guid PurchaseOrderId { get; set; }
        public string PONumber { get; set; } = default!;
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = default!;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Status { get; set; } = default!;
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Notes { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class CreatePurchaseOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int QuantityOrdered { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; } = 0;
        public decimal Discount { get; set; } = 0;
    }

    public class CreatePurchaseOrderDto
    {
        public Guid SupplierId { get; set; }
        public Guid WarehouseId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string? Notes { get; set; }
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
    }

    // Goods Receiving ke liye — jitni actual quantity mili usi se stock badhta hai
    public class ReceiveGoodsItemDto
    {
        public Guid PurchaseOrderItemId { get; set; }
        public int QuantityReceived { get; set; }
        public int QuantityDamaged { get; set; } = 0;
        public int QuantityRejected { get; set; } = 0;
    }

    public class ReceiveGoodsDto
    {
        public List<ReceiveGoodsItemDto> Items { get; set; } = new();
    }
}