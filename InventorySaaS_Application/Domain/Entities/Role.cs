using System;
using System.Collections.Generic;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Role
    {
        public Guid RoleId { get; set; } = Guid.NewGuid();
        
        public Guid? TenantId { get; set; } // null = system-wide role
        
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}