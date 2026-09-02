using System;

namespace InventorySaaS_Application.Application.DTOs
{
    public class CategoryDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = default!;
        public Guid? ParentCategoryId { get; set; }
        public string Status { get; set; } = default!;
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = default!;
        public Guid? ParentCategoryId { get; set; }
    }

    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = default!;
        public string SKU { get; set; } = default!;
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? BrandId { get; set; }
        public string UnitOfMeasure { get; set; } = default!;
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public int ReorderLevel { get; set; }
        public string? ImageUrl { get; set; }
        public string Status { get; set; } = default!;
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = default!;
        public string SKU { get; set; } = default!;
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public string UnitOfMeasure { get; set; } = "Piece";
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public Guid? TaxId { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public int ReorderLevel { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
    }
}