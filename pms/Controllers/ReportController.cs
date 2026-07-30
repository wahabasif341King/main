using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportController(AppDbContext context)
    {
        _context = context;
    }

    // Ek project ka progress report
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetProjectReport(int projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return NotFound("Project not found");

        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

        var total = tasks.Count;
        var done = tasks.Count(t => t.Status == "done");
        var progressPercent = total == 0 ? 0 : Math.Round((double)done / total * 100, 2);

        var report = new
        {
            ProjectTitle = project.Title,
            TotalTasks = total,
            CompletedTasks = done,
            InProgressTasks = tasks.Count(t => t.Status == "in-progress"),
            TodoTasks = tasks.Count(t => t.Status == "todo"),
            ProgressPercent = progressPercent
        };

        return Ok(report);
    }

    // Har member ne kitne tasks complete kiye (workload report)
    [HttpGet("project/{projectId}/workload")]
    public async Task<IActionResult> GetWorkloadReport(int projectId)
    {
        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == projectId && t.AssignedToId != null)
            .Include(t => t.AssignedUser)
            .ToListAsync();

        var workload = tasks
            .GroupBy(t => t.AssignedUser.Username)
            .Select(g => new
            {
                UserName = g.Key,
                TotalAssigned = g.Count(),
                Completed = g.Count(t => t.Status == "done"),
                Pending = g.Count(t => t.Status != "done")
            })
            .ToList();

        return Ok(workload);
    }
}