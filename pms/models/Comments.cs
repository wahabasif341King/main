using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; }

    [ForeignKey("Task")]
    public int TaskId { get; set; }
    public TaskItem Task { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}