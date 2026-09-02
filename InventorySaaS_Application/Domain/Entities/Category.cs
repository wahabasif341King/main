
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; } // FK -> Organizations.OrganizationId

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        public Guid? ParentCategoryId { get; set; } // self-reference, nullable
        public Category? ParentCategory { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // Active / Inactive

        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}