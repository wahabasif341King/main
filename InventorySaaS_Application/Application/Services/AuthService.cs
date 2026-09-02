using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InventorySaaS_Application.Application.DTOs;
using InventorySaaS_Application.Domain.Entities;
using InventorySaaS_Application.Infrastructure.Data;


namespace InventorySaaS_Application.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IJwtService _jwtService;

        public AuthService(AppDbContext db, IJwtService jwtService)
        {
            _db = db;
            _jwtService = jwtService;
        }

        // Business signup -> Organization (tenant) + Company Admin banata hai
        public async Task<AuthResponseDto> RegisterOrganizationAsync(RegisterOrganizationDto dto)
        {
            var emailExists = await _db.Users.AnyAsync(u => u.Email == dto.AdminEmail);
            if (emailExists)
                throw new InvalidOperationException("An account with this email already exists.");


            // Ya to registration ka poora process successfully complete hoga, ya agar beech mein error aya to changes save nahi honge.
            // Is code mein multiple database operations hain:

            // Organization create
            // User create
            // UserRole create

            // Agar pehle do save ho gaye aur teesre step mein error aa gaya, to transaction na ho to database inconsistent ho sakta hai.
            // Transaction ki wajah se ideally sab operations ek unit ki tarah handle hote hain.

            using var transaction = await _db.Database.BeginTransactionAsync();

            var organization = new Organization
            {
                Name = dto.OrganizationName,
                Currency = dto.Currency ?? "PKR",
                Status = "Active"
            };
            _db.Organizations.Add(organization);
            await _db.SaveChangesAsync();

            var adminRole = await _db.Roles
                .FirstOrDefaultAsync(r => r.Name == "Company Admin" && r.TenantId == null);

            if (adminRole == null)
                throw new InvalidOperationException(
                    "'Company Admin' role not found. Did you run the EF Core migration?");

            var user = new User
            {
                TenantId = organization.OrganizationId,
                FullName = dto.AdminFullName,
                Email = dto.AdminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Status = "Active"
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _db.UserRoles.Add(new UserRole
            {
                TenantId = organization.OrganizationId,
                UserId = user.UserId,
                RoleId = adminRole.RoleId
            });
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
            
            //Iska matlab:
            // Registration ke jitne bhi database changes hue hain unko permanently confirm / save kar do.

            // Agar yahan tak koi error nahi aya, to:

            // Organization successfully create ✓
            // User successfully create ✓
            // Company Admin role successfully assign ✓

            // Transaction commit ho jayegi.

            return await BuildAuthResponseAsync(user, new List<string> { adminRole.Name });

            // Yahan Ab response de raha hai.
            // Jis mein user ki detail, user ka role diya ja raha hai.
        }

        // Same tenant ke andar naya employee (Manager/Salesperson/etc.) add karta hai
        public async Task<AuthResponseDto> RegisterEmployeeAsync(Guid tenantId, RegisterEmployeeDto dto)
        {
            var emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                throw new InvalidOperationException("A user with this email already exists.");

            var allowedRoles = new[] { "Company Admin", "Manager", "Salesperson", "Warehouse Staff", "Accountant" };
            if (!allowedRoles.Contains(dto.RoleName))
                throw new InvalidOperationException(
                    $"Invalid role. Allowed roles: {string.Join(", ", allowedRoles)}");

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == dto.RoleName && r.TenantId == null);
            if (role == null)
                throw new InvalidOperationException($"Role '{dto.RoleName}' does not exist.");

            var user = new User
            {
                TenantId = tenantId,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Status = "Active"
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _db.UserRoles.Add(new UserRole
            {
                TenantId = tenantId,
                UserId = user.UserId,
                RoleId = role.RoleId
            });
            await _db.SaveChangesAsync();

            return await BuildAuthResponseAsync(user, new List<string> { role.Name });
        }

        // Login - har role ke liye same tarah kaam karta hai
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (user.Status != "Active")
                throw new UnauthorizedAccessException("This account is not active.");

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            return await BuildAuthResponseAsync(user, roles);
        }

        private async Task<AuthResponseDto> BuildAuthResponseAsync(User user, List<string> roles)
        {
            var (accessToken, expiry) = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _db.SaveChangesAsync();

            return new AuthResponseDto
            {
                UserId = user.UserId,
                TenantId = user.TenantId,
                FullName = user.FullName,
                Email = user.Email,
                Roles = roles,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = expiry
            };
        }
    }
}