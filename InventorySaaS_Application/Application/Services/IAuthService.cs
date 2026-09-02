
using System;
using System.Threading.Tasks;
using InventorySaaS_Application.Application.DTOs;

namespace InventorySaaS_Application.Application.Services
{
    // Interface batata hai ke class ko kya kya cheezen provide karni hongi, lekin usually ye nahi batata ke un cheezon ka actual code kaise likhna hai.
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterOrganizationAsync(RegisterOrganizationDto dto);
        
        Task<AuthResponseDto> RegisterEmployeeAsync(Guid tenantId, RegisterEmployeeDto dto);
        
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}