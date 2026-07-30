using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    public class Project 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; } = "active"; // active, completed, archived

        [ForeignKey("Owner")]
        public int OwnerId { get; set; }
        public User Owner { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TaskItem> Tasks { get; set; }
        public ICollection<ProjectMember> Members { get; set; }
    }

    // Many-to-many: Project <-> User (members)
    public class ProjectMember
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }


