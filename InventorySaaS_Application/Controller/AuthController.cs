
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventorySaaS_Application.Application.DTOs;
using InventorySaaS_Application.Application.Services;

namespace InventorySaaS_Application.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST api/auth/register
        // Business signup: naya Organization (tenant) + Company Admin banata hai
        [HttpPost("register")]
        public async Task<IActionResult> RegisterOrganization(RegisterOrganizationDto dto)
        {
            try
            {
                var result = await _authService.RegisterOrganizationAsync(dto);
                SetAuthCookie(result);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST api/auth/register-employee
        // Valid JWT chahiye. Same tenant ke andar naya user add karta hai.
        [Authorize]
        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee(RegisterEmployeeDto dto)
        {
            try
            {
                var tenantIdClaim = User.FindFirstValue("TenantId");
                if (string.IsNullOrEmpty(tenantIdClaim))
                    return Unauthorized();

                var tenantId = Guid.Parse(tenantIdClaim);
                var result = await _authService.RegisterEmployeeAsync(tenantId, dto);
                SetAuthCookie(result);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST api/auth/login
        // Har role isi endpoint se login karega
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                SetAuthCookie(result);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // POST api/auth/logout
        // AccessToken cookie clear kar deta hai
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AccessToken");
            return Ok(new { message = "Logged out." });
        }

        // GET api/auth/me
        // Token test karne ke liye (protected endpoint)
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub),
                Email = User.FindFirstValue(JwtRegisteredClaimNames.Email),
                TenantId = User.FindFirstValue("TenantId"),
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value)
            });
        }

        // AccessToken ko HttpOnly cookie mein daal deta hai taake browser
        // response body ke ilawa cookie mein bhi token save kare aur
        // agli requests mein automatically bhej sake.
        private void SetAuthCookie(AuthResponseDto result)
        {
            Response.Cookies.Append("AccessToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,          // HTTPS required (backend already forces HTTPS redirect)
                SameSite = SameSiteMode.None, // frontend alag origin/port par hai (cross-site)
                Expires = result.AccessTokenExpiry
            });
        }
    }
}