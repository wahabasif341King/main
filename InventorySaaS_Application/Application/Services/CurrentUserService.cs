using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InventorySaaS_Application.Application.Services
{
    // Har request ke JWT token se TenantId aur UserId nikalta hai.
    // CategoryService, ProductService, WarehouseService, InventoryService,
    // waghera — sab isi ke through pata karte hain ke "current" user kaun hai
    // aur kis tenant se belong karta hai (tenant isolation isi pe base hai).
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid TenantId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue("TenantId");
                if (string.IsNullOrEmpty(value))
                    throw new UnauthorizedAccessException("TenantId claim not found in token.");
                return Guid.Parse(value);
            }
        }

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (string.IsNullOrEmpty(value))
                    throw new UnauthorizedAccessException("User id claim not found in token.");
                return Guid.Parse(value);
            }
        }
    }
}