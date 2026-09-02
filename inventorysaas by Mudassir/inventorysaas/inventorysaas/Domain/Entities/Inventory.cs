using System;
using System.ComponentModel.DataAnnotations;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Inventory
    {
        [Key]
        public Guid InventoryId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public Guid? VariantId { get; set; }
        public ProductVariant? Variant { get; set; }

        [Required]
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = default!;

        public int QuantityAvailable { get; set; }
        public int QuantityReserved { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}