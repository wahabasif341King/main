using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc; // Is Library se ye functions Use hotey hain: 
// ControllerBase
// ApiController
// Route
// HttpPost
// Ok()
// BadRequest()
// Unauthorized()
// IActionResult


[Route("api/auth")] // Ye atrribute batat hai ke kis URL pr ye chale ga.
[ApiController]  // Ye sirf API banata hai. Ye json fromat mein response dega.
public class AuthController : ControllerBase // AuthController ek Controller hai and ControllerBase inheritance means AuthController ControllerBase ki functionality le raha hai.
// ControllerBase ki Functionality mein hai:
// Ok()
// BadRequest()
// Unauthorized()
// NotFound()   , etc.
{
    private readonly AppDbContext _db; // Is mein MongoDB se User Collection _user mein store ho rai hai.
    private readonly TokenService _tokenService; // Ye Login krne pr Jwt Token banaye ga

    public AuthController(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService; // TokenService ko store kr liya
    }

    // POST: /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        var existing = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (existing != null) return BadRequest("Username already taken.");

        var user = new User
        {
            Username = dto.Username,
            Email_Address = dto.Email_Address,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Username == "admin" ? "Admin" : "User"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Registered successfully!" });
    }

    // POST: /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email_Address == dto.Email_Adress);
        if (user == null) return Unauthorized("Invalid username or password.");

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isPasswordValid) return Unauthorized("Invalid username or password.");

        var token = _tokenService.GenerateToken(user);
        return Ok(new { token });
    }

    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize] // Ye attribute batata hai ke ye function sirf authorized user ke liye hai. Agr user authorized nai
    public IActionResult Logout()
    {
        // JWT stateless hota hai, server ke paas koi session store nahi hoti
        // Isliye "logout" ka matlab hai: client apna stored token delete kar de
        return Ok(new { message = "Logged out successfully. Please delete your token on client side." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email_Address == dto.Email);
        if (user == null)
        {
            // Security practice: mat batao ke email exist nahi karta (email enumeration attack se bachne ke liye)
            return Ok(new { message = "If this email exists, a reset link has been sent." });
        }

        // Simple reset token generate karo (real app mein yeh email se bhejte hain)
        var resetToken = Guid.NewGuid().ToString();

        user.ResetToken = resetToken;
        user.ResetTokenExpiry = DateTime.Now.AddMinutes(15);

        await _db.SaveChangesAsync();

        // Real app mein: yahan email service se resetToken bhejo user ko
        // Abhi ke liye response mein hi wapas bhej rahe hain (testing ke liye)
        return Ok(new { message = "Reset token generated.", resetToken });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.ResetToken == dto.Token);
        if (user == null || user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.Now)
            return BadRequest("Invalid or expired reset token.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Password reset successfully." });
    }
}