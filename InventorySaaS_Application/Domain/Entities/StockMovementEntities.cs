using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventorySaaS_Application.Domain.Entities
{
    public class StockMovement
    {
        [Key]
        public Guid StockMovementId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public Guid? VariantId { get; set; }

        [Required]
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = default!;

        [Required, MaxLength(30)]
        public string MovementType { get; set; } = default!;
        // Purchase / Sale / SalesReturn / PurchaseReturn / Transfer / Adjustment / Damage / OpeningStock

        public int QuantityChange { get; set; } // +ve or -ve

        [MaxLength(30)]
        public string? ReferenceType { get; set; } // SalesOrder / PurchaseOrder / Transfer / Adjustment

        public Guid? ReferenceId { get; set; }

        [Required]
        public Guid CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class StockAdjustment
    {
        [Key]
        public Guid StockAdjustmentId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        [Required]
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = default!;

        public int PreviousQuantity { get; set; }
        public int NewQuantity { get; set; }
        public int Difference { get; set; } // NewQuantity - PreviousQuantity

        [MaxLength(150)]
        public string? Reason { get; set; }

        [Required]
        public Guid AdjustedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class StockTransfer
    {
        [Key]
        public Guid StockTransferId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid FromWarehouseId { get; set; }
        public Warehouse FromWarehouse { get; set; } = default!;

        [Required]
        public Guid ToWarehouseId { get; set; }
        public Warehouse ToWarehouse { get; set; } = default!;

        [MaxLength(20)]
        public string Status { get; set; } = "Draft";
        // Draft / Requested / Approved / InTransit / Received

        [Required]
        public Guid RequestedByUserId { get; set; }
        public Guid? ApprovedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
    }

    public class StockTransferItem
    {
        [Key]
        public Guid StockTransferItemId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid StockTransferId { get; set; }
        public StockTransfer StockTransfer { get; set; } = default!;

        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public Guid? VariantId { get; set; }

        public int Quantity { get; set; }
    }
}