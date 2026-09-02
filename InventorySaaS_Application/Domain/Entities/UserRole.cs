using System;

namespace InventorySaaS_Application.Domain.Entities
{
    public class UserRole
    {
        public Guid UserRoleId { get; set; } = Guid.NewGuid();
        
        public Guid TenantId { get; set; }
        
        public Guid UserId { get; set; }
        
        public Guid RoleId { get; set; }


        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}