
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;




    [ApiController]
    [Route("api/projects/{projectId}/tasks")]
[Authorize]
public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(int projectId, CreateTaskDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                AssignedToId = dto.AssignedToId,
                Priority = dto.Priority ?? "medium",
                DueDate = dto.DueDate,
                ProjectId = projectId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Agar task assign hua hai, notification banao
            if (dto.AssignedToId.HasValue)
            {
                var notification = new Notification
                {
                    Message = $"You have been assigned a new task: {task.Title}",
                    Type = "task-assigned",
                    UserId = dto.AssignedToId.Value
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            return Ok(task);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksByProject(
     int projectId,
     [FromQuery] string? status,
     [FromQuery] string? priority,
     [FromQuery] string? search,
     [FromQuery] int? assignedToId)
        {
            var query = _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.AssignedUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(t => t.Priority == priority);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Title.Contains(search) || t.Description.Contains(search));

            if (assignedToId.HasValue)
                query = query.Where(t => t.AssignedToId == assignedToId.Value);

            var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
            return Ok(tasks);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto dto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound("Task not found");

            task.Title = dto.Title ?? task.Title;
            task.Description = dto.Description ?? task.Description;
            task.Status = dto.Status ?? task.Status;
            task.Priority = dto.Priority ?? task.Priority;
            task.AssignedToId = dto.AssignedToId ?? task.AssignedToId;

            await _context.SaveChangesAsync();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound("Task not found");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Task deleted successfully" });
        }
    }
