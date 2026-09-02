

using System;
using System.Collections.Generic;
using InventorySaaS_Application.Domain.Entities;

namespace InventorySaaS_Application.Application.Services
{
    public interface IJwtService
    {
        (string token, DateTime expiry) GenerateAccessToken(User user, List<string> roles);
        string GenerateRefreshToken();
    }
}