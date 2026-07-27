using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pms.Data;
using pms.DTOs;

namespace pms.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId() => 1; // temporary, JWT ready hone tak

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var userId = GetUserId();

        var myProjects = _context.Projects
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId));

        var projectIds = await myProjects.Select(p => p.Id).ToListAsync();

        var myTasks = _context.Tasks.Where(t => projectIds.Contains(t.ProjectId));

        var summary = new DashboardSummaryDto
        {
            TotalProjects = await myProjects.CountAsync(),
            ActiveProjects = await myProjects.CountAsync(p => p.Status == "active"),
            CompletedProjects = await myProjects.CountAsync(p => p.Status == "completed"),
            TotalTasks = await myTasks.CountAsync(),
            TasksTodo = await myTasks.CountAsync(t => t.Status == "todo"),
            TasksInProgress = await myTasks.CountAsync(t => t.Status == "in-progress"),
            TasksDone = await myTasks.CountAsync(t => t.Status == "done"),
            OverdueTasks = await myTasks.CountAsync(t =>
                t.DueDate != null && t.DueDate < DateTime.UtcNow && t.Status != "done")
        };

        return Ok(summary);
    }

    // Recent activity: latest 5 tasks aur projects
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentActivity()
    {
        var userId = GetUserId();

        var recentProjects = await _context.Projects
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .ToListAsync();

        var projectIds = recentProjects.Select(p => p.Id).ToList();

        var recentTasks = await _context.Tasks
            .Where(t => projectIds.Contains(t.ProjectId))
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .ToListAsync();

        return Ok(new { recentProjects, recentTasks });
    }
}