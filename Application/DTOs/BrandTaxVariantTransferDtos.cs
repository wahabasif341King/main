using System;
using System.Collections.Generic;

namespace InventorySaaS_Application.Application.DTOs
{
    public class BrandDto
    {
        public Guid BrandId { get; set; }
        public string Name { get; set; } = default!;
        public string Status { get; set; } = default!;
    }

    public class CreateBrandDto
    {
        public string Name { get; set; } = default!;
    }

    public class TaxDto
    {
        public Guid TaxId { get; set; }
        public string Name { get; set; } = default!;
        public decimal Percentage { get; set; }
        public string AppliesTo { get; set; } = default!;
    }

    public class CreateTaxDto
    {
        public string Name { get; set; } = default!;
        public decimal Percentage { get; set; }
        public string AppliesTo { get; set; } = "Product";
    }

    public class ProductVariantDto
    {
        public Guid VariantId { get; set; }
        public Guid ProductId { get; set; }
        public string SKU { get; set; } = default!;
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
    }

    public class CreateProductVariantDto
    {
        public Guid ProductId { get; set; }
        public string SKU { get; set; } = default!;
        public string? Barcode { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class StockTransferDto
    {
        public Guid StockTransferId { get; set; }
        public Guid FromWarehouseId { get; set; }
        public string? FromWarehouseName { get; set; }
        public Guid ToWarehouseId { get; set; }
        public string? ToWarehouseName { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public List<StockTransferItemDto> Items { get; set; } = new();
    }

    public class StockTransferItemDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateStockTransferDto
    {
        public Guid FromWarehouseId { get; set; }
        public Guid ToWarehouseId { get; set; }
        public List<CreateStockTransferItemDto> Items { get; set; } = new();
    }

    public class CreateStockTransferItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}