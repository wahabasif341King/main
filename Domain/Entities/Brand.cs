
using System;
using System.ComponentModel.DataAnnotations;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Brand
    {
        [Key]
        public Guid BrandId { get; set; } = Guid.NewGuid();
     
        [Required]
        public Guid TenantId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}