using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    public int Id { get; set; } // // MySQL mein auto-increment int hota hai, MongoDB ke ObjectId string ki tarah nahi

    [Required]
    [StringLength(100)]
    public string? Username { get; set; }

    [Required]
    [EmailAddress]
    public string? Email_Address { get; set; }

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
}