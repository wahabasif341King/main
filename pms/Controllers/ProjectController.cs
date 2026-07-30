
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



[ApiController]
    [Route("api/projects")]
[Authorize]
// Person 1 ka JWT auth yahan lagega
public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

    private int GetUserId() =>
     int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectDto dto)
        {
            var project = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                OwnerId = GetUserId()
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return Ok(project);
        }

    [HttpGet]
    public async Task<IActionResult> GetAllProjects(
 [FromQuery] string? search,
 [FromQuery] string? status,
 [FromQuery] string? sortBy,
 [FromQuery] bool descending = false)
    {
        var userId = GetUserId();

        var query = _context.Projects
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
            .Include(p => p.Owner)
            .AsQueryable();

        // Search by title or description
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p =>
                p.Title.Contains(search) ||
                p.Description.Contains(search));
        }

        // Filter by status
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(p => p.Status == status);
        }

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "title" => descending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
            "duedate" => descending ? query.OrderByDescending(p => p.DueDate) : query.OrderBy(p => p.DueDate),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var projects = await query.ToListAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound("Project not found");
            return Ok(project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound("Project not found");

            project.Title = dto.Title ?? project.Title;
            project.Description = dto.Description ?? project.Description;
            project.Status = dto.Status ?? project.Status;
            project.DueDate = dto.DueDate ?? project.DueDate;

            await _context.SaveChangesAsync();
            return Ok(project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound("Project not found");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Project deleted successfully" });
        }
    }
