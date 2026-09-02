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
        private readonly AppDBContext _context;
        private readonly ICurrentUserService _currentUser;

        public BrandService(AppDBContext context, ICurrentUserService currentUser)
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

        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            var brand = new Brand { TenantId = _currentUser.TenantId, Name = dto.Name, Status = "Active" };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return new BrandDto { BrandId = brand.BrandId, Name = brand.Name, Status = brand.Status };
        }
    }

    public class TaxService
    {
        private readonly AppDBContext _context;
        private readonly ICurrentUserService _currentUser;

        public TaxService(AppDBContext context, ICurrentUserService currentUser)
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
    }

    public class ProductVariantService
    {
        private readonly AppDBContext _context;
        private readonly ICurrentUserService _currentUser;

        public ProductVariantService(AppDBContext context, ICurrentUserService currentUser)
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
    }
}