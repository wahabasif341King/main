
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/profile")]
[ApiController]
[Authorize]  // Koi bhi logged-in user (Admin ya User) apna profile access kar sakta hai
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProfileController(AppDbContext db)
    {
        _db = db;
    }

    // JWT claims se current user ka Id nikalta hai (ab int mein convert karna hoga)
    private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // GET: /api/profile — apna profile dekho
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound("Profile not found.");

        return Ok(new { user.Id, user.Username, user.Email_Address, user.Role });
    }

    // PUT: /api/profile — apna profile update karo (sirf Email, Username — Role khud change nahi kar sakta)
    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var userId = GetCurrentUserId();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound("Profile not found.");

        user.Username = dto.Username ?? user.Username;
        user.Email_Address = dto.Email ?? user.Email_Address;

        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Profile updated successfully." });
    }

    // PUT: /api/profile/change-password
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = GetCurrentUserId();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound("Profile not found.");

        // Pehle purana password verify karo
        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            return BadRequest("Old password is incorrect.");

        var newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.PasswordHash = newHash;

        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Password changed successfully." });
    }
}