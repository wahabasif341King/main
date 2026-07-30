
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

[Route("api/users")]
[ApiController]
[Authorize(Roles = "Admin")]  // Sirf Admin is poore Controller ko access kar sakta hai
public class UserController : ControllerBase
{
    private readonly AppDbContext _db;

    public UserController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /api/users — sab users dekho
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.Users.ToListAsync();

        // PasswordHash kabhi bhi response mein wapas mat bhejo — security risk
        var safeDataUsers = users.Select(u => new
        {
            u.Id,
            u.Username,
            u.Email_Address,
            u.Role
        });

        return Ok(safeDataUsers);
    }

    // GET: /api/users/{id} — ek specific user dekho
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound("User not found.");

        return Ok(new { user.Id, user.Username, user.Email_Address, user.Role });
    }

    // PUT: /api/users/{id} — user ka Role ya Email update karo
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateUserDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound("User not found.");

        user.Email_Address = dto.Email ?? user.Email_Address;
        user.Role = dto.Role ?? user.Role;

        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return Ok(new { message = "User updated successfully." });
    }

    // DELETE: /api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound("User not found.");

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return Ok(new { message = "User deleted successfully." });
    }
}