using System;
using System.Collections.Generic;

namespace InventorySaaS_Application.Application.DTOs
{
    public class SalesOrderItemDto
    {
        public Guid SalesOrderItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }

    public class SalesOrderDto
    {
        public Guid SalesOrderId { get; set; }
        public string OrderNumber { get; set; } = default!;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = default!;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = default!;
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public List<SalesOrderItemDto> Items { get; set; } = new();
    }

    public class CreateSalesOrderItemDto
    {
        public Guid ProductId { get; set; }
        public Guid? VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; } = 0;
        public decimal Tax { get; set; } = 0;
    }

    public class CreateSalesOrderDto
    {
        public Guid CustomerId { get; set; }
        public Guid WarehouseId { get; set; }
        public decimal ShippingAmount { get; set; } = 0;
        public List<CreateSalesOrderItemDto> Items { get; set; } = new();
    }
}