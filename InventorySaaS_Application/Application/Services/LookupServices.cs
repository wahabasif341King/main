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
    public class BrandService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public BrandService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<BrandDto>> GetAllAsync()
        {
            return await _context.Brands
                .Where(b => b.TenantId == _currentUser.TenantId)
                .Select(b => new BrandDto { BrandId = b.BrandId, Name = b.Name, Status = b.Status })
                .ToListAsync();
        }

        public async Task<BrandDto?> GetByIdAsync(Guid BrandId)
        {
            var brand = await _context.Brands
                .FirstOrDefaultAsync(b =>
                    b.BrandId == BrandId &&
                    b.TenantId == _currentUser.TenantId);

            if (brand == null)
                return null;

            return new BrandDto
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                Status = brand.Status
            };
        }

        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            var brand = new Brand { TenantId = _currentUser.TenantId, Name = dto.Name, Status = "Active" };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return new BrandDto { BrandId = brand.BrandId, Name = brand.Name, Status = brand.Status };
        }

        public async Task<BrandDto?> UpdateAsync(Guid BrandId, CreateBrandDto dto)
        {
            var brand = await _context.Brands
                .FirstOrDefaultAsync(b =>
                    b.BrandId == BrandId &&
                    b.TenantId == _currentUser.TenantId);

            if (brand == null)
                return null;

            brand.Name = dto.Name;

            await _context.SaveChangesAsync();

            return new BrandDto
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                Status = brand.Status
            };
        }

        public async Task<bool> DeleteAsync(Guid BrandId)
        {
            var brand = await _context.Brands
                .FirstOrDefaultAsync(b =>
                    b.BrandId == BrandId &&
                    b.TenantId == _currentUser.TenantId);

            if (brand == null)
                return false;

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return true;
        }
    }

    public class TaxService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public TaxService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<TaxDto>> GetAllAsync()
        {
            return await _context.Taxes
                .Where(t => t.TenantId == _currentUser.TenantId)
                .Select(t => new TaxDto
                {
                    TaxId = t.TaxId,
                    Name = t.Name,
                    Percentage = t.Percentage,
                    AppliesTo = t.AppliesTo
                })
                .ToListAsync();
        }

        public async Task<TaxDto?> GetByIdAsync(Guid TaxId)
        {
            var tax = await _context.Taxes
                .FirstOrDefaultAsync(t =>
                    t.TaxId == TaxId &&
                    t.TenantId == _currentUser.TenantId);

            if (tax == null)
                return null;

            return new TaxDto
            {
                TaxId = tax.TaxId,
                Name = tax.Name,
                Percentage = tax.Percentage,
                AppliesTo = tax.AppliesTo
            };
        }

        public async Task<TaxDto> CreateAsync(CreateTaxDto dto)
        {
            var tax = new Tax
            {
                TenantId = _currentUser.TenantId,
                Name = dto.Name,
                Percentage = dto.Percentage,
                AppliesTo = dto.AppliesTo
            };
            _context.Taxes.Add(tax);
            await _context.SaveChangesAsync();
            return new TaxDto { TaxId = tax.TaxId, Name = tax.Name, Percentage = tax.Percentage, AppliesTo = tax.AppliesTo };
        }

        public async Task<TaxDto?> UpdateAsync(Guid TaxId, CreateTaxDto dto)
        {
            var tax = await _context.Taxes
                .FirstOrDefaultAsync(t =>
                    t.TaxId == TaxId &&
                    t.TenantId == _currentUser.TenantId);

            if (tax == null)
                return null;

            tax.Name = dto.Name;
            tax.Percentage = dto.Percentage;
            tax.AppliesTo = dto.AppliesTo;

            await _context.SaveChangesAsync();

            return new TaxDto
            {
                TaxId = tax.TaxId,
                Name = tax.Name,
                Percentage = tax.Percentage,
                AppliesTo = tax.AppliesTo
            };
        }

        public async Task<bool> DeleteAsync(Guid TaxId)
        {
            var tax = await _context.Taxes
                .FirstOrDefaultAsync(t =>
                    t.TaxId == TaxId &&
                    t.TenantId == _currentUser.TenantId);

            if (tax == null)
                return false;

            // Product.TaxId -> Tax has no explicit SetNull/Restrict config, so
            // guard here instead of letting a stray FK error surface as a 500.
            var inUse = await _context.Products.AnyAsync(p =>
                p.TenantId == _currentUser.TenantId && p.TaxId == TaxId);
            if (inUse)
                throw new InvalidOperationException(
                    "This tax is assigned to one or more products and cannot be deleted.");

            _context.Taxes.Remove(tax);
            await _context.SaveChangesAsync();

            return true;
        }
    }

    public class ProductVariantService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public ProductVariantService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<ProductVariantDto>> GetByProductAsync(Guid productId)
        {
            return await _context.ProductVariants
                .Where(v => v.TenantId == _currentUser.TenantId && v.ProductId == productId)
                .Select(v => new ProductVariantDto
                {
                    VariantId = v.VariantId,
                    ProductId = v.ProductId,
                    SKU = v.SKU,
                    Color = v.Color,
                    Size = v.Size,
                    Price = v.Price,
                    Cost = v.Cost
                })
                .ToListAsync();
        }

        public async Task<ProductVariantDto?> GetByIdAsync(Guid VariantId)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v =>
                    v.VariantId == VariantId &&
                    v.TenantId == _currentUser.TenantId);

            if (variant == null)
                return null;

            return new ProductVariantDto
            {
                VariantId = variant.VariantId,
                ProductId = variant.ProductId,
                SKU = variant.SKU,
                Color = variant.Color,
                Size = variant.Size,
                Price = variant.Price,
                Cost = variant.Cost
            };
        }

        public async Task<ProductVariantDto> CreateAsync(CreateProductVariantDto dto)
        {
            var exists = await _context.Products.AnyAsync(p =>
                p.TenantId == _currentUser.TenantId && p.ProductId == dto.ProductId);
            if (!exists)
                throw new InvalidOperationException("Product not found.");

            var variant = new ProductVariant
            {
                TenantId = _currentUser.TenantId,
                ProductId = dto.ProductId,
                SKU = dto.SKU,
                Barcode = dto.Barcode,
                Color = dto.Color,
                Size = dto.Size,
                Price = dto.Price,
                Cost = dto.Cost,
                ImageUrl = dto.ImageUrl
            };
            _context.ProductVariants.Add(variant);

            var product = await _context.Products.FirstAsync(p => p.ProductId == dto.ProductId);
            product.HasVariants = true;

            await _context.SaveChangesAsync();

            return new ProductVariantDto
            {
                VariantId = variant.VariantId,
                ProductId = variant.ProductId,
                SKU = variant.SKU,
                Color = variant.Color,
                Size = variant.Size,
                Price = variant.Price,
                Cost = variant.Cost
            };
        }

        public async Task<ProductVariantDto?> UpdateAsync(Guid VariantId, CreateProductVariantDto dto)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v =>
                    v.VariantId == VariantId &&
                    v.TenantId == _currentUser.TenantId);

            if (variant == null)
                return null;

            variant.SKU = dto.SKU;
            variant.Barcode = dto.Barcode;
            variant.Color = dto.Color;
            variant.Size = dto.Size;
            variant.Price = dto.Price;
            variant.Cost = dto.Cost;
            variant.ImageUrl = dto.ImageUrl;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(VariantId);
        }

        public async Task<bool> DeleteAsync(Guid VariantId)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v =>
                    v.VariantId == VariantId &&
                    v.TenantId == _currentUser.TenantId);

            if (variant == null)
                return false;

            var productId = variant.ProductId;

            _context.ProductVariants.Remove(variant);

            await _context.SaveChangesAsync();

            // Check whether this was the last variant of the product
            var hasVariants = await _context.ProductVariants
                .AnyAsync(v =>
                    v.ProductId == productId &&
                    v.TenantId == _currentUser.TenantId);

            // If no variants remain, set HasVariants = false
            if (!hasVariants)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p =>
                        p.ProductId == productId &&
                        p.TenantId == _currentUser.TenantId);

                if (product != null)
                {
                    product.HasVariants = false;
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }
    }
}