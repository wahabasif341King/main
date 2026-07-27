// Controllers/UserController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

[Route("api/users")]
[ApiController]
[Authorize(Roles = "Admin")]  // Sirf Admin is poore Controller ko access kar sakta hai
public class UserController : ControllerBase
{
    private readonly IMongoCollection<User> _users;

    public UserController(MongoDBContext context)
    {
        _users = context.Users;
    }

    // GET: /api/users — sab users dekho
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _users.Find(_ => true).ToListAsync();

        // PasswordHash kabhi bhi response mein wapas mat bhejo — security risk
        var safeUsers = users.Select(u => new
        {
            u.Id,
            u.Username,
            u.Email_Address,
            u.Role
        });

        return Ok(safeUsers);
    }

    // GET: /api/users/{id} — ek specific user dekho
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        if (user == null) return NotFound("User not found.");

        return Ok(new { user.Id, user.Username, user.Email_Address, user.Role });
    }

    // PUT: /api/users/{id} — user ka Role ya Email update karo
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateUserDto dto)
    {
        var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        if (user == null) return NotFound("User not found.");

        var update = Builders<User>.Update
            .Set(u => u.Email_Address, dto.Email ?? user.Email_Address)
            .Set(u => u.Role, dto.Role ?? user.Role);

        await _users.UpdateOneAsync(u => u.Id == id, update);
        return Ok(new { message = "User updated successfully." });
    }

    // DELETE: /api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == id);
        if (result.DeletedCount == 0) return NotFound("User not found.");

        return Ok(new { message = "User deleted successfully." });
    }
}