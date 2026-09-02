using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySaaS_Application.Domain.Entities
{
    public class ProductVariant
    {
        [Key]
        public Guid VariantId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        [Required, MaxLength(50)]
        public string SKU { get; set; } = default!;

        [MaxLength(50)]
        public string? Barcode { get; set; }

        [MaxLength(30)]
        public string? Color { get; set; }

        [MaxLength(20)]
        public string? Size { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }
    }
}