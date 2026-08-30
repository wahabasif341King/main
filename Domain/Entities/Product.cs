
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(50)]
        public string SKU { get; set; } = default!;

        [MaxLength(50)]
        public string? Barcode { get; set; }

        public string? Description { get; set; } // TEXT

        [Required]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        public Guid? BrandId { get; set; }
        public Brand? Brand { get; set; }

        [MaxLength(20)]
        public string UnitOfMeasure { get; set; } = "Piece";

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }

        public Guid? TaxId { get; set; }
        public Tax? Tax { get; set; }

        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public int ReorderLevel { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Weight { get; set; }

        [MaxLength(50)]
        public string? Dimensions { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // Active / Inactive / Discontinued

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}