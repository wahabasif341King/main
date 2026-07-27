namespace pms.DTOs;
    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AssignedToId { get; set; }
        public string? Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int? AssignedToId { get; set; }
    }

