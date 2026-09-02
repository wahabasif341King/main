using System;
using System.Collections.Generic;

namespace InventorySaaS_Application.Domain.Entities
{
    public class User
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        
        public Guid TenantId { get; set; }
        
        public string FullName { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string PasswordHash { get; set; } = string.Empty;
        
        public string? PhoneNumber { get; set; }
        
        public string? RefreshToken { get; set; }
        
        public DateTime? RefreshTokenExpiry { get; set; }
        
        public string Status { get; set; } = "Active";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public Organization Organization { get; set; } = null!;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}