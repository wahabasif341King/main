// Controllers/ProfileController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

[Route("api/profile")]
[ApiController]
[Authorize]  // Koi bhi logged-in user (Admin ya User) apna profile access kar sakta hai
public class ProfileController : ControllerBase
{
    private readonly IMongoCollection<User> _users;

    public ProfileController(MongoDBContext context)
    {
        _users = context.Users;
    }

    private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // GET: /api/profile — apna profile dekho
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        if (user == null) return NotFound("Profile not found.");

        return Ok(new { user.Id, user.Username, user.Email_Address, user.Role });
    }

    // PUT: /api/profile — apna profile update karo (sirf Email, Username — Role khud change nahi kar sakta)
    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var userId = GetCurrentUserId();
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        if (user == null) return NotFound("Profile not found.");

        var update = Builders<User>.Update
            .Set(u => u.Username, dto.Username ?? user.Username)
            .Set(u => u.Email_Address, dto.Email ?? user.Email_Address);

        await _users.UpdateOneAsync(u => u.Id == userId, update);
        return Ok(new { message = "Profile updated successfully." });
    }

    // PUT: /api/profile/change-password
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = GetCurrentUserId();
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        if (user == null) return NotFound("Profile not found.");

        // Pehle purana password verify karo
        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            return BadRequest("Old password is incorrect.");

        var newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        var update = Builders<User>.Update.Set(u => u.PasswordHash, newHash);

        await _users.UpdateOneAsync(u => u.Id == userId, update);
        return Ok(new { message = "Password changed successfully." });
    }
}