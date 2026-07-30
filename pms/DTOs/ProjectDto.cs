

    public class CreateProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
    public List<int>? MemberIds { get; set; } = new();
    }

    public class UpdateProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
    }
