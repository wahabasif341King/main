using System.ComponentModel.DataAnnotations;

public class RegisterDTO
{
    [Required]
    public string? Username = string.Empty;


    [Required, EmailAddress]
    public string? Email_Address = string.Empty;

    [Required, MinLength(8)]
    public string? Password = string.Empty;

}

public class LoginDTO
{
    [Required, EmailAddress]
    public string? Email_Adress = string.Empty;

    [Required]
    public string? Password = string.Empty;
}

public class UpdateUserDto
{
    [EmailAddress]
    public string? Email { get; set; }

    public string? Role { get; set; } // "Admin" ya "User"
}

public class UpdateProfileDto
{
    public string? Username { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}

public class ChangePasswordDto
{
    [Required]
    public string OldPassword { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}

public class ForgotPasswordDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}