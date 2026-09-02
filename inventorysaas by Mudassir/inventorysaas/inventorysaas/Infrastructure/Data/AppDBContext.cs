using InventorySaaS_Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Reflection.Emit;

namespace InventorySaaS_Application.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // --- Auth (his tables) ---
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        // --- Catalog (your tables) ---
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Tax> Taxes => Set<Tax>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

        // --- Warehouse & Inventory (your tables) ---
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();
        public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
        public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
        public DbSet<StockTransferItem> StockTransferItems => Set<StockTransferItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===================== Organizations =====================
            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(o => o.OrganizationId);
                entity.Property(o => o.Name).HasMaxLength(150).IsRequired();
            });

            // ===================== Users =====================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.FullName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
                entity.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();

                // Per-tenant uniqueness + global uniqueness.
                // Login currently finds user by Email alone, tenant isn't known yet.
                entity.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.HasOne(u => u.Organization)
                    .WithMany(o => o.Users)
                    .HasForeignKey(u => u.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===================== Roles =====================
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleId);
                entity.Property(r => r.Name).HasMaxLength(50).IsRequired();
            });

            // ===================== UserRoles =====================
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => ur.UserRoleId);

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Default roles seeded into the database
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111101"), TenantId = null, Name = "Super Admin", Description = "Platform owner" },
                new Role { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111102"), TenantId = null, Name = "Company Admin", Description = "Business owner/admin" },
                new Role { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111103"), TenantId = null, Name = "Manager", Description = "Manages operations" },
                new Role { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111104"), TenantId = null, Name = "Salesperson", Description = "Handles sales orders" },
                new Role { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111105"), TenantId = null, Name = "Warehouse Staff", Description = "Handles inventory" },
                new Role { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111106"), TenantId = null, Name = "Accountant", Description = "Handles payments/invoices" }
            );

            // ===================== Catalog =====================
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.TenantId, p.SKU })
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProductVariant>()
                .HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===================== Warehouse & Inventory =====================
            modelBuilder.Entity<Warehouse>()
                .HasIndex(w => new { w.TenantId, w.Code })
                .IsUnique();

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Warehouse)
                .WithMany()
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockTransfer>()
                .HasOne(t => t.FromWarehouse)
                .WithMany()
                .HasForeignKey(t => t.FromWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockTransfer>()
                .HasOne(t => t.ToWarehouse)
                .WithMany()
                .HasForeignKey(t => t.ToWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockTransferItem>()
                .HasOne(i => i.StockTransfer)
                .WithMany(t => t.Items)
                .HasForeignKey(i => i.StockTransferId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}