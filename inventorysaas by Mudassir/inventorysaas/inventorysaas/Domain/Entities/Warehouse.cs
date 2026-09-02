using System;
using System.ComponentModel.DataAnnotations;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Warehouse
    {
        [Key]
        public Guid WarehouseId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [MaxLength(20)]
        public string? Code { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        public Guid? ManagerUserId { get; set; } // FK -> Users.UserId (his table)

        [MaxLength(20)]
        public string Status { get; set; } = "Active";
    }
}