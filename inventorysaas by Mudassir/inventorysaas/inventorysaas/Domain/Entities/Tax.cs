using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Tax
    {
        [Key]
        public Guid TaxId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = default!; // GST / VAT / Sales Tax

        [Column(TypeName = "decimal(5,2)")]
        public decimal Percentage { get; set; }

        [MaxLength(20)]
        public string AppliesTo { get; set; } = "Product"; // Product / Customer / Order
    }
}