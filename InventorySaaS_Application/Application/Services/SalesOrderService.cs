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
    public class SalesOrderService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        private static readonly string[] ValidStatuses =
            { "Draft", "Confirmed", "Processing", "Packed", "Shipped", "Delivered", "Cancelled" };

        public SalesOrderService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<SalesOrderDto>> GetAllAsync()
        {
            return await _context.SalesOrders
                .Where(o => o.TenantId == _currentUser.TenantId)
                .Include(o => o.Customer)
                .Include(o => o.Warehouse)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new SalesOrderDto
                {
                    SalesOrderId = o.SalesOrderId,
                    OrderNumber = o.OrderNumber,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.Name,
                    WarehouseId = o.WarehouseId,
                    WarehouseName = o.Warehouse.Name,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    Subtotal = o.Subtotal,
                    DiscountAmount = o.DiscountAmount,
                    TaxAmount = o.TaxAmount,
                    ShippingAmount = o.ShippingAmount,
                    GrandTotal = o.GrandTotal,
                    Items = o.Items.Select(i => new SalesOrderItemDto
                    {
                        SalesOrderItemId = i.SalesOrderItemId,
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        Discount = i.Discount,
                        Tax = i.Tax,
                        Total = i.Total
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<SalesOrderDto?> GetByIdAsync(Guid orderId)
        {
            return (await GetAllAsync()).FirstOrDefault(o => o.SalesOrderId == orderId);
        }

        public async Task<SalesOrderDto> CreateAsync(CreateSalesOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("At least one item is required.");

            var customerExists = await _context.Customers.AnyAsync(c =>
                c.CustomerId == dto.CustomerId && c.TenantId == _currentUser.TenantId);
            if (!customerExists)
                throw new InvalidOperationException("Customer not found.");

            var warehouseExists = await _context.Warehouses.AnyAsync(w =>
                w.WarehouseId == dto.WarehouseId && w.TenantId == _currentUser.TenantId);
            if (!warehouseExists)
                throw new InvalidOperationException("Warehouse not found.");

            var order = new SalesOrder
            {
                TenantId = _currentUser.TenantId,
                OrderNumber = await GenerateOrderNumberAsync(),
                CustomerId = dto.CustomerId,
                WarehouseId = dto.WarehouseId,
                OrderDate = DateTime.UtcNow,
                Status = "Draft",
                ShippingAmount = dto.ShippingAmount,
                CreatedByUserId = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow
            };

            decimal subtotal = 0, discountTotal = 0, taxTotal = 0;

            foreach (var item in dto.Items)
            {
                var lineTotal = (item.Price * item.Quantity) - item.Discount + item.Tax;
                subtotal += item.Price * item.Quantity;
                discountTotal += item.Discount;
                taxTotal += item.Tax;

                order.Items.Add(new SalesOrderItem
                {
                    ProductId = item.ProductId,
                    VariantId = item.VariantId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Discount = item.Discount,
                    Tax = item.Tax,
                    Total = lineTotal
                });
            }

            order.Subtotal = subtotal;
            order.DiscountAmount = discountTotal;
            order.TaxAmount = taxTotal;
            order.GrandTotal = subtotal - discountTotal + taxTotal + order.ShippingAmount;

            _context.SalesOrders.Add(order);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(order.SalesOrderId)
                ?? throw new InvalidOperationException("Failed to load created order.");
        }

        public async Task<SalesOrderDto> UpdateStatusAsync(Guid orderId, string newStatus)
        {
            if (!ValidStatuses.Contains(newStatus))
                throw new InvalidOperationException($"Invalid status '{newStatus}'.");

            var order = await _context.SalesOrders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.SalesOrderId == orderId && o.TenantId == _currentUser.TenantId);

            if (order == null)
                throw new InvalidOperationException("Sales order not found.");

            // Jab order PEHLI baar "Confirmed" hota hai, tabhi warehouse se stock kam hota hai
            if (newStatus == "Confirmed" && order.Status != "Confirmed")
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                foreach (var item in order.Items)
                {
                    var inventory = await _context.Inventories.FirstOrDefaultAsync(i =>
                        i.TenantId == _currentUser.TenantId &&
                        i.ProductId == item.ProductId &&
                        i.WarehouseId == order.WarehouseId &&
                        i.VariantId == null);

                    if (inventory == null)
                    {
                        inventory = new Inventory
                        {
                            TenantId = _currentUser.TenantId,
                            ProductId = item.ProductId,
                            WarehouseId = order.WarehouseId,
                            QuantityAvailable = 0
                        };
                        _context.Inventories.Add(inventory);
                    }

                    inventory.QuantityAvailable -= item.Quantity;
                    inventory.LastUpdated = DateTime.UtcNow;

                    _context.StockMovements.Add(new StockMovement
                    {
                        TenantId = _currentUser.TenantId,
                        ProductId = item.ProductId,
                        WarehouseId = order.WarehouseId,
                        MovementType = "Sale",
                        QuantityChange = -item.Quantity,
                        ReferenceType = "SalesOrder",
                        ReferenceId = order.SalesOrderId,
                        CreatedByUserId = _currentUser.UserId,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                order.ApprovedByUserId ??= _currentUser.UserId;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return await GetByIdAsync(order.SalesOrderId)
                ?? throw new InvalidOperationException("Failed to reload order.");
        }

        private async Task<string> GenerateOrderNumberAsync()
        {
            var count = await _context.SalesOrders.CountAsync(o => o.TenantId == _currentUser.TenantId);
            return $"ORD-{(count + 1):D5}"; // e.g. ORD-00001
        }
    }
}