using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace pms.Controllers;


[ApiController]
[Route("api/tasks/{taskId}/comments")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateComment(int taskId, CreateCommentDto dto)
    {
        var taskExists = await _context.Tasks.AnyAsync(t => t.Id == taskId);
        if (!taskExists) return NotFound("Task not found");

        var comment = new Comment
        {
            Content = dto.Content,
            TaskId = taskId,
            UserId = GetUserId()
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return Ok(comment);
    }

    [HttpGet]
    public async Task<IActionResult> GetCommentsByTask(int taskId)
    {
        var comments = await _context.Comments
            .Where(c => c.TaskId == taskId)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return Ok(comments);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, UpdateCommentDto dto)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null) return NotFound("Comment not found");

        comment.Content = dto.Content;
        await _context.SaveChangesAsync();
        return Ok(comment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null) return NotFound("Comment not found");

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Comment deleted successfully" });
    }
}