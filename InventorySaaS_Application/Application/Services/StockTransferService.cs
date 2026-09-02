using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InventorySaaS_Application.Domain.Entities;
using InventorySaaS_Application.Application.DTOs;
using InventorySaaS_Application.Infrastructure.Data;

namespace InventorySaaS_Application.Application.Services
{
    public class StockTransferService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public StockTransferService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<StockTransferDto>> GetAllAsync()
        {
            return await _context.StockTransfers
                .Where(t => t.TenantId == _currentUser.TenantId)
                .Include(t => t.FromWarehouse)
                .Include(t => t.ToWarehouse)
                .Include(t => t.Items).ThenInclude(i => i.Product)
                .Select(t => new StockTransferDto
                {
                    StockTransferId = t.StockTransferId,
                    FromWarehouseId = t.FromWarehouseId,
                    FromWarehouseName = t.FromWarehouse.Name,
                    ToWarehouseId = t.ToWarehouseId,
                    ToWarehouseName = t.ToWarehouse.Name,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    Items = t.Items.Select(i => new StockTransferItemDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<StockTransferDto> CreateAsync(CreateStockTransferDto dto)
        {
            if (dto.FromWarehouseId == dto.ToWarehouseId)
                throw new InvalidOperationException("From and To warehouse can't be the same.");

            var transfer = new StockTransfer
            {
                TenantId = _currentUser.TenantId,
                FromWarehouseId = dto.FromWarehouseId,
                ToWarehouseId = dto.ToWarehouseId,
                Status = "Draft",
                RequestedByUserId = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in dto.Items)
            {
                transfer.Items.Add(new StockTransferItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            _context.StockTransfers.Add(transfer);
            await _context.SaveChangesAsync();

            return (await GetAllAsync()).First(t => t.StockTransferId == transfer.StockTransferId);
        }

        public async Task<StockTransferDto> UpdateStatusAsync(Guid transferId, string newStatus)
        {
            var validStatuses = new[] { "Draft", "Requested", "Approved", "InTransit", "Received" };
            if (!validStatuses.Contains(newStatus))
                throw new InvalidOperationException($"Invalid status '{newStatus}'.");

            var transfer = await _context.StockTransfers
                .Include(t => t.Items)
                .FirstOrDefaultAsync(t => t.StockTransferId == transferId && t.TenantId == _currentUser.TenantId);

            if (transfer == null)
                throw new InvalidOperationException("Transfer not found.");

            if (newStatus == "Received" && transfer.Status != "Received")
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                foreach (var item in transfer.Items)
                {
                    var fromInventory = await _context.Inventories.FirstOrDefaultAsync(i =>
                        i.TenantId == _currentUser.TenantId &&
                        i.ProductId == item.ProductId &&
                        i.WarehouseId == transfer.FromWarehouseId &&
                        i.VariantId == null);

                    if (fromInventory == null)
                    {
                        fromInventory = new Inventory
                        {
                            TenantId = _currentUser.TenantId,
                            ProductId = item.ProductId,
                            WarehouseId = transfer.FromWarehouseId,
                            QuantityAvailable = 0
                        };
                        _context.Inventories.Add(fromInventory);
                    }
                    fromInventory.QuantityAvailable -= item.Quantity;
                    fromInventory.LastUpdated = DateTime.UtcNow;

                    _context.StockMovements.Add(new StockMovement
                    {
                        TenantId = _currentUser.TenantId,
                        ProductId = item.ProductId,
                        WarehouseId = transfer.FromWarehouseId,
                        MovementType = "TransferOut",
                        QuantityChange = -item.Quantity,
                        ReferenceType = "Transfer",
                        ReferenceId = transfer.StockTransferId,
                        CreatedByUserId = _currentUser.UserId,
                        CreatedAt = DateTime.UtcNow
                    });

                    var toInventory = await _context.Inventories.FirstOrDefaultAsync(i =>
                        i.TenantId == _currentUser.TenantId &&
                        i.ProductId == item.ProductId &&
                        i.WarehouseId == transfer.ToWarehouseId &&
                        i.VariantId == null);

                    if (toInventory == null)
                    {
                        toInventory = new Inventory
                        {
                            TenantId = _currentUser.TenantId,
                            ProductId = item.ProductId,
                            WarehouseId = transfer.ToWarehouseId,
                            QuantityAvailable = 0
                        };
                        _context.Inventories.Add(toInventory);
                    }
                    toInventory.QuantityAvailable += item.Quantity;
                    toInventory.LastUpdated = DateTime.UtcNow;

                    _context.StockMovements.Add(new StockMovement
                    {
                        TenantId = _currentUser.TenantId,
                        ProductId = item.ProductId,
                        WarehouseId = transfer.ToWarehouseId,
                        MovementType = "TransferIn",
                        QuantityChange = item.Quantity,
                        ReferenceType = "Transfer",
                        ReferenceId = transfer.StockTransferId,
                        CreatedByUserId = _currentUser.UserId,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                transfer.ApprovedByUserId ??= _currentUser.UserId;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            transfer.Status = newStatus;
            await _context.SaveChangesAsync();

            return (await GetAllAsync()).First(t => t.StockTransferId == transfer.StockTransferId);
        }
    }
}