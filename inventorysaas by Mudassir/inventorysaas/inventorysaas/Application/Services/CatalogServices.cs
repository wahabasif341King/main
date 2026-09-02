
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
                .Where(c => c.TenantId == _currentUser.TenantId)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId,
                    Status = c.Status
                })
                .ToListAsync();
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

        public async Task<bool> DeleteAsync(Guid categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.TenantId == _currentUser.TenantId);

            if (category == null) return false;

            category.Status = "Inactive"; // soft delete — don't hard-delete, Products FK to it
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
                .Where(p => p.TenantId == _currentUser.TenantId)
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
    }
}