using System;

namespace InventorySaaS_Application.Application.DTOs
{
    public class WarehouseDto
    {
        public Guid WarehouseId { get; set; }
        public string Name { get; set; } = default!;
        public string? Code { get; set; }
        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public Guid? ManagerUserId { get; set; }
        public string Status { get; set; } = default!;
    }

    public class CreateWarehouseDto
    {
        public string Name { get; set; } = default!;
        public string? Code { get; set; }
        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public Guid? ManagerUserId { get; set; }
    }

    public class InventoryDto
    {
        public Guid InventoryId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductSKU { get; set; }
        public Guid? VariantId { get; set; }
        public Guid WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantityReserved { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    // Used for the initial "opening stock" entry, and for manual stock adjustments.
    // Never lets the caller set QuantityAvailable directly — always goes through
    // a movement so StockMovement stays a true audit trail.
    public class StockAdjustmentRequestDto
    {
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public int PhysicalQuantity { get; set; } // what was actually counted
        public string? Reason { get; set; }
    }

    public class StockMovementDto
    {
        public Guid StockMovementId { get; set; }
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public string MovementType { get; set; } = default!;
        public int QuantityChange { get; set; }
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}