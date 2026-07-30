

public class DashboardSummaryDto
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int TotalTasks { get; set; }
    public int TasksTodo { get; set; }
    public int TasksInProgress { get; set; }
    public int TasksDone { get; set; }
    public int OverdueTasks { get; set; }
}