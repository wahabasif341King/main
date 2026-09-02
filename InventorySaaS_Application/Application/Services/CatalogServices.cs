
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
    // NOTE: ITenantProvider/ICurrentUserService is whatever your teammate already
    // built for auth to expose the logged-in user's TenantId (usually reads it
    // from JWT claims via HttpContext). Ask him what it's called in his code —
    // it might already exist as ICurrentUserService or similar. Swap the name
    // below to match his, don't create a second, competing one.
    public interface ICurrentUserService
    {
        Guid TenantId { get; }
        Guid UserId { get; }
    }
    // ICurrentUserService ye ensure karta hai ke us organization ke employees fetch hon jis Organization ko Login kiya hua hai.

    public class CategoryService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CategoryService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .Where(c => c.TenantId == _currentUser.TenantId && c.Status == "Active")
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId,
                    Status = c.Status
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == categoryId &&
                    c.TenantId == _currentUser.TenantId);

            if (category == null)
                return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                Status = category.Status
            };
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                TenantId = _currentUser.TenantId,
                Name = dto.Name,
                ParentCategoryId = dto.ParentCategoryId,
                Status = "Active"
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                Status = category.Status
            };
        }

        public async Task<CategoryDto?> UpdateAsync(Guid categoryId, CreateCategoryDto dto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == categoryId &&
                    c.TenantId == _currentUser.TenantId);

            if (category == null)
                return null;

            category.Name = dto.Name;
            category.ParentCategoryId = dto.ParentCategoryId;

            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                Status = category.Status
            };
        }

        public async Task<bool> DeleteAsync(Guid categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == categoryId &&
                    c.TenantId == _currentUser.TenantId);

            if (category == null)
                return false;

            // Soft delete — hard delete crashes if this category still has
            // Products or SubCategories pointing at it (Restrict FK).
            category.Status = "Inactive";

            await _context.SaveChangesAsync();

            return true;
        }
    }

    public class ProductService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public ProductService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            return await _context.Products
                .Where(p => p.TenantId == _currentUser.TenantId && p.Status == "Active")
                .Include(p => p.Category)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    SKU = p.SKU,
                    Barcode = p.Barcode,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    BrandId = p.BrandId,
                    UnitOfMeasure = p.UnitOfMeasure,
                    CostPrice = p.CostPrice,
                    SellingPrice = p.SellingPrice,
                    MinimumStock = p.MinimumStock,
                    MaximumStock = p.MaximumStock,
                    ReorderLevel = p.ReorderLevel,
                    ImageUrl = p.ImageUrl,
                    Status = p.Status
                })
                .ToListAsync();
        }

        public async Task<ProductDto?> GetByIdAsync(Guid productId)
        {
            var p = await _context.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.TenantId == _currentUser.TenantId);

            if (p == null) return null;

            return new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                SKU = p.SKU,
                Barcode = p.Barcode,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                BrandId = p.BrandId,
                UnitOfMeasure = p.UnitOfMeasure,
                CostPrice = p.CostPrice,
                SellingPrice = p.SellingPrice,
                MinimumStock = p.MinimumStock,
                MaximumStock = p.MaximumStock,
                ReorderLevel = p.ReorderLevel,
                ImageUrl = p.ImageUrl,
                Status = p.Status
            };
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            // SKU must be unique per tenant — check before insert

            var exists = await _context.Products.AnyAsync(p =>
                p.TenantId == _currentUser.TenantId && p.SKU == dto.SKU);
            if (exists)
                throw new InvalidOperationException($"SKU '{dto.SKU}' already exists.");

            var product = new Product
            {
                TenantId = _currentUser.TenantId,
                Name = dto.Name,
                SKU = dto.SKU,
                Barcode = dto.Barcode,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId,
                UnitOfMeasure = dto.UnitOfMeasure,
                CostPrice = dto.CostPrice,
                SellingPrice = dto.SellingPrice,
                TaxId = dto.TaxId,
                MinimumStock = dto.MinimumStock,
                MaximumStock = dto.MaximumStock,
                ReorderLevel = dto.ReorderLevel,
                ImageUrl = dto.ImageUrl,
                Weight = dto.Weight,
                Dimensions = dto.Dimensions,
                Status = "Active"
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // A new product with zero stock everywhere is normal — Inventory rows
            // get created per-warehouse the first time stock is received/adjusted,
            // not automatically here.

            return (await GetByIdAsync(product.ProductId))!;
        }

        public async Task<ProductDto?> UpdateAsync(Guid productId, CreateProductDto dto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductId == productId &&
                    p.TenantId == _currentUser.TenantId);

            if (product == null)
                return null;

            // SKU duplicate check(SKU stands for Stock keeping unit)
            // SKU ki help se aap:
            // Product ko uniquely identify kar sakte ho.
            // Stock track kar sakte ho.
            // Same name wale products ko distinguish kar sakte ho.
            // Sales aur inventory management easy bana sakte ho.
            
            var skuExists = await _context.Products.AnyAsync(p =>
                p.TenantId == _currentUser.TenantId &&
                p.SKU == dto.SKU &&
                p.ProductId != productId);
            // Ye poochna
            if (skuExists)
                throw new InvalidOperationException(
                    $"SKU '{dto.SKU}' already exists.");

            product.Name = dto.Name;
            product.SKU = dto.SKU;
            product.Barcode = dto.Barcode;
            product.Description = dto.Description;
            product.CategoryId = dto.CategoryId;
            product.BrandId = dto.BrandId;
            product.UnitOfMeasure = dto.UnitOfMeasure;
            product.CostPrice = dto.CostPrice;
            product.SellingPrice = dto.SellingPrice;
            product.TaxId = dto.TaxId;
            product.MinimumStock = dto.MinimumStock;
            product.MaximumStock = dto.MaximumStock;
            product.ReorderLevel = dto.ReorderLevel;
            product.ImageUrl = dto.ImageUrl;
            product.Weight = dto.Weight;
            product.Dimensions = dto.Dimensions;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.ProductId);
        }

        public async Task<bool> DeleteAsync(Guid productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductId == productId &&
                    p.TenantId == _currentUser.TenantId);

            if (product == null)
                return false;

            // Soft delete — Inventory has a Restrict FK on ProductId, so once
            // any stock record exists a hard delete would throw a DB error.
            product.Status = "Inactive";

            await _context.SaveChangesAsync();

            return true;
        }
    }
}