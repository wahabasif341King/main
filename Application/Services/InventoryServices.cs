
using InventorySaaS_Application.Application.DTOs;
using InventorySaaS_Application.Domain.Entities;
using InventorySaaS_Application.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySaaS_Application.Application.Services
{
    public class WarehouseService
    {
        private readonly AppDBContext _context;
        private readonly ICurrentUserService _currentUser;

        public WarehouseService(AppDBContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<WarehouseDto>> GetAllAsync()
        {
            return await _context.Warehouses
                .Where(w => w.TenantId == _currentUser.TenantId)
                .Select(w => new WarehouseDto
                {
                    WarehouseId = w.WarehouseId,
                    Name = w.Name,
                    Code = w.Code,
                    Address = w.Address,
                    ContactNumber = w.ContactNumber,
                    ManagerUserId = w.ManagerUserId,
                    Status = w.Status
                })
                .ToListAsync();
        }

        public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto)
        {
            var warehouse = new Warehouse
            {
                TenantId = _currentUser.TenantId,
                Name = dto.Name,
                Code = dto.Code,
                Address = dto.Address,
                ContactNumber = dto.ContactNumber,
                ManagerUserId = dto.ManagerUserId,
                Status = "Active"
            };

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            return new WarehouseDto
            {
                WarehouseId = warehouse.WarehouseId,
                Name = warehouse.Name,
                Code = warehouse.Code,
                Address = warehouse.Address,
                ContactNumber = warehouse.ContactNumber,
                ManagerUserId = warehouse.ManagerUserId,
                Status = warehouse.Status
            };
        }
    }

    public class InventoryService
    {
        private readonly AppDBContext _context;
        private readonly ICurrentUserService _currentUser;

        public InventoryService(AppDBContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<InventoryDto>> GetAllAsync(Guid? warehouseId = null)
        {
            var query = _context.Inventories
                .Where(i => i.TenantId == _currentUser.TenantId)
                .Include(i => i.Product)
                .Include(i => i.Warehouse)
                .AsQueryable();

            if (warehouseId.HasValue)
                query = query.Where(i => i.WarehouseId == warehouseId.Value);

            return await query.Select(i => new InventoryDto
            {
                InventoryId = i.InventoryId,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductSKU = i.Product.SKU,
                VariantId = i.VariantId,
                WarehouseId = i.WarehouseId,
                WarehouseName = i.Warehouse.Name,
                QuantityAvailable = i.QuantityAvailable,
                QuantityReserved = i.QuantityReserved,
                LastUpdated = i.LastUpdated
            }).ToListAsync();
        }

        /// <summary>
        /// The one method that's allowed to change stock numbers. Everything else
        /// (sales, purchases, transfers, later) should route through this so
        /// StockMovement always stays accurate. Takes a physical count and
        /// reconciles it against the system quantity.
        /// </summary>
        public async Task<InventoryDto> AdjustStockAsync(StockAdjustmentRequestDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var inventory = await _context.Inventories.FirstOrDefaultAsync(i =>
                i.TenantId == _currentUser.TenantId &&
                i.ProductId == dto.ProductId &&
                i.WarehouseId == dto.WarehouseId &&
                i.VariantId == null);

            int previousQuantity = inventory?.QuantityAvailable ?? 0;
            int difference = dto.PhysicalQuantity - previousQuantity;

            if (inventory == null)
            {
                // first time this product has a stock record in this warehouse
                inventory = new Inventory
                {
                    TenantId = _currentUser.TenantId,
                    ProductId = dto.ProductId,
                    WarehouseId = dto.WarehouseId,
                    QuantityAvailable = dto.PhysicalQuantity,
                    QuantityReserved = 0,
                    LastUpdated = DateTime.UtcNow
                };
                _context.Inventories.Add(inventory);
            }
            else
            {
                inventory.QuantityAvailable = dto.PhysicalQuantity;
                inventory.LastUpdated = DateTime.UtcNow;
            }

            // audit trail — never skip this
            var adjustment = new StockAdjustment
            {
                TenantId = _currentUser.TenantId,
                ProductId = dto.ProductId,
                WarehouseId = dto.WarehouseId,
                PreviousQuantity = previousQuantity,
                NewQuantity = dto.PhysicalQuantity,
                Difference = difference,
                Reason = dto.Reason,
                AdjustedByUserId = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow
            };
            _context.StockAdjustments.Add(adjustment);

            var movement = new StockMovement
            {
                TenantId = _currentUser.TenantId,
                ProductId = dto.ProductId,
                WarehouseId = dto.WarehouseId,
                MovementType = "Adjustment",
                QuantityChange = difference,
                ReferenceType = "Adjustment",
                ReferenceId = adjustment.StockAdjustmentId,
                CreatedByUserId = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow
            };
            _context.StockMovements.Add(movement);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new InventoryDto
            {
                InventoryId = inventory.InventoryId,
                ProductId = inventory.ProductId,
                WarehouseId = inventory.WarehouseId,
                QuantityAvailable = inventory.QuantityAvailable,
                QuantityReserved = inventory.QuantityReserved,
                LastUpdated = inventory.LastUpdated
            };
        }
    }
}