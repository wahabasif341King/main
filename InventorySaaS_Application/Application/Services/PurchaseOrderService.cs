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
    public class PurchaseOrderService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly AuditLogService _auditLog;

        private static readonly string[] ValidStatuses =
            { "Draft", "Sent", "PartiallyReceived", "Received", "Cancelled" };

        public PurchaseOrderService(AppDbContext context, ICurrentUserService currentUser, AuditLogService auditLog)
        {
            _context = context;
            _currentUser = currentUser;
            _auditLog = auditLog;
        }

        public async Task<List<PurchaseOrderDto>> GetAllAsync()
        {
            return await _context.PurchaseOrders
                .Where(o => o.TenantId == _currentUser.TenantId)
                .Include(o => o.Supplier)
                .Include(o => o.Warehouse)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new PurchaseOrderDto
                {
                    PurchaseOrderId = o.PurchaseOrderId,
                    PONumber = o.PONumber,
                    SupplierId = o.SupplierId,
                    SupplierName = o.Supplier.Name,
                    WarehouseId = o.WarehouseId,
                    WarehouseName = o.Warehouse.Name,
                    OrderDate = o.OrderDate,
                    ExpectedDeliveryDate = o.ExpectedDeliveryDate,
                    Status = o.Status,
                    Subtotal = o.Subtotal,
                    DiscountAmount = o.DiscountAmount,
                    TaxAmount = o.TaxAmount,
                    GrandTotal = o.GrandTotal,
                    Notes = o.Notes,
                    Items = o.Items.Select(i => new PurchaseOrderItemDto
                    {
                        PurchaseOrderItemId = i.PurchaseOrderItemId,
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        QuantityOrdered = i.QuantityOrdered,
                        QuantityReceived = i.QuantityReceived,
                        QuantityDamaged = i.QuantityDamaged,
                        QuantityRejected = i.QuantityRejected,
                        Price = i.Price,
                        Tax = i.Tax,
                        Discount = i.Discount
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<PurchaseOrderDto?> GetByIdAsync(Guid id)
        {
            return (await GetAllAsync()).FirstOrDefault(o => o.PurchaseOrderId == id);
        }

        public async Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("At least one item is required.");

            var supplierExists = await _context.Suppliers.AnyAsync(s =>
                s.SupplierId == dto.SupplierId && s.TenantId == _currentUser.TenantId);
            if (!supplierExists)
                throw new InvalidOperationException("Supplier not found.");

            var warehouseExists = await _context.Warehouses.AnyAsync(w =>
                w.WarehouseId == dto.WarehouseId && w.TenantId == _currentUser.TenantId);
            if (!warehouseExists)
                throw new InvalidOperationException("Warehouse not found.");

            var order = new PurchaseOrder
            {
                TenantId = _currentUser.TenantId,
                PONumber = await GeneratePONumberAsync(),
                SupplierId = dto.SupplierId,
                WarehouseId = dto.WarehouseId,
                OrderDate = DateTime.UtcNow,
                ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
                Notes = dto.Notes,
                Status = "Draft",
                CreatedByUserId = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow
            };

            decimal subtotal = 0, taxTotal = 0, discountTotal = 0;

            foreach (var item in dto.Items)
            {
                subtotal += item.Price * item.QuantityOrdered;
                taxTotal += item.Tax;
                discountTotal += item.Discount;

                order.Items.Add(new PurchaseOrderItem
                {
                    ProductId = item.ProductId,
                    QuantityOrdered = item.QuantityOrdered,
                    Price = item.Price,
                    Tax = item.Tax,
                    Discount = item.Discount
                });
            }

            order.Subtotal = subtotal;
            order.TaxAmount = taxTotal;
            order.DiscountAmount = discountTotal;
            order.GrandTotal = subtotal - discountTotal + taxTotal;

            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync("Created", "PurchaseOrder", order.PurchaseOrderId, newValue: order.PONumber);

            return await GetByIdAsync(order.PurchaseOrderId)
                ?? throw new InvalidOperationException("Failed to load created purchase order.");
        }

        public async Task<PurchaseOrderDto> UpdateStatusAsync(Guid id, string newStatus)
        {
            if (!ValidStatuses.Contains(newStatus))
                throw new InvalidOperationException($"Invalid status '{newStatus}'.");

            var order = await _context.PurchaseOrders
                .FirstOrDefaultAsync(o => o.PurchaseOrderId == id && o.TenantId == _currentUser.TenantId);

            if (order == null)
                throw new InvalidOperationException("Purchase order not found.");

            var oldStatus = order.Status;
            order.Status = newStatus;
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync("StatusChanged", "PurchaseOrder", order.PurchaseOrderId, oldStatus, newStatus);

            return await GetByIdAsync(id) ?? throw new InvalidOperationException("Failed to reload order.");
        }

        // Goods Receiving — jitni ACTUAL quantity aayi utna hi stock badhta hai
        public async Task<PurchaseOrderDto> ReceiveGoodsAsync(Guid purchaseOrderId, ReceiveGoodsDto dto)
        {
            var order = await _context.PurchaseOrders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.PurchaseOrderId == purchaseOrderId && o.TenantId == _currentUser.TenantId);

            if (order == null)
                throw new InvalidOperationException("Purchase order not found.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            foreach (var receiveItem in dto.Items)
            {
                var poItem = order.Items.FirstOrDefault(i => i.PurchaseOrderItemId == receiveItem.PurchaseOrderItemId);
                if (poItem == null)
                    throw new InvalidOperationException("Purchase order item not found.");

                poItem.QuantityReceived += receiveItem.QuantityReceived;
                poItem.QuantityDamaged += receiveItem.QuantityDamaged;
                poItem.QuantityRejected += receiveItem.QuantityRejected;

                if (receiveItem.QuantityReceived > 0)
                {
                    var inventory = await _context.Inventories.FirstOrDefaultAsync(i =>
                        i.TenantId == _currentUser.TenantId &&
                        i.ProductId == poItem.ProductId &&
                        i.WarehouseId == order.WarehouseId &&
                        i.VariantId == null);

                    if (inventory == null)
                    {
                        inventory = new Inventory
                        {
                            TenantId = _currentUser.TenantId,
                            ProductId = poItem.ProductId,
                            WarehouseId = order.WarehouseId,
                            QuantityAvailable = 0
                        };
                        _context.Inventories.Add(inventory);
                    }

                    inventory.QuantityAvailable += receiveItem.QuantityReceived;
                    inventory.LastUpdated = DateTime.UtcNow;

                    _context.StockMovements.Add(new StockMovement
                    {
                        TenantId = _currentUser.TenantId,
                        ProductId = poItem.ProductId,
                        WarehouseId = order.WarehouseId,
                        MovementType = "Purchase",
                        QuantityChange = receiveItem.QuantityReceived,
                        ReferenceType = "PurchaseOrder",
                        ReferenceId = order.PurchaseOrderId,
                        CreatedByUserId = _currentUser.UserId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Overall PO status determine karo: sab items poori tarah reconcile ho gaye to "Received",
            // kuch ho gaye to "PartiallyReceived"
            var allFullyProcessed = order.Items.All(i =>
                i.QuantityReceived + i.QuantityDamaged + i.QuantityRejected >= i.QuantityOrdered);
            var anyProcessed = order.Items.Any(i => i.QuantityReceived + i.QuantityDamaged + i.QuantityRejected > 0);

            order.Status = allFullyProcessed ? "Received" : anyProcessed ? "PartiallyReceived" : order.Status;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await _auditLog.LogAsync("GoodsReceived", "PurchaseOrder", order.PurchaseOrderId, newValue: order.Status);

            return await GetByIdAsync(purchaseOrderId) ?? throw new InvalidOperationException("Failed to reload order.");
        }

        private async Task<string> GeneratePONumberAsync()
        {
            var count = await _context.PurchaseOrders.CountAsync(o => o.TenantId == _currentUser.TenantId);
            return $"PO-{(count + 1):D5}";
        }
    }
}