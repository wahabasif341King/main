using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pms.Models;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Message { get; set; }

    public string Type { get; set; } = "info"; // info, task-assigned, comment, project-update

    [ForeignKey("User")]
    public int UserId { get; set; } // kis user ko notification jani hai
    public User User { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}