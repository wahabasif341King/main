

using System;
using System.Collections.Generic;

namespace InventorySaaS_Application.Application.DTOs
{
    public class AuthResponseDto
    {
        public Guid UserId { get; set; }
        
        public Guid TenantId { get; set; }
        
        public string FullName { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public List<string> Roles { get; set; } = new();
        
        public string AccessToken { get; set; } = string.Empty;
        
        public string RefreshToken { get; set; } = string.Empty;
        
        public DateTime AccessTokenExpiry { get; set; }
    }
}