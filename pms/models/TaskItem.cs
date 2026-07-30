using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [ForeignKey("AssignedUser")]
        public int? AssignedToId { get; set; }
        public User AssignedUser { get; set; }

        public string Status { get; set; } = "todo"; // todo, in-progress, done
        public string Priority { get; set; } = "medium"; // low, medium, high

        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
