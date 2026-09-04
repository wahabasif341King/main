using InventorySaaS_Application.Application.DTOs;
using InventorySaaS_Application.Domain.Entities;
using InventorySaaS_Application.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySaaS_Application.Application.Services
{
    public class AuditLogService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public AuditLogService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // Koi bhi service ye call kar ke ek "kis ne kya kiya" record dal sakti hai
        public async Task LogAsync(string action, string entityName, Guid entityId, string? oldValue = null, string? newValue = null)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                TenantId = _currentUser.TenantId,
                UserId = _currentUser.UserId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                OldValue = oldValue,
                NewValue = newValue,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuditLogDto>> GetAllAsync()
        {
            return await _context.AuditLogs
                .Where(a => a.TenantId == _currentUser.TenantId)
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(500) // recent 500 — table bohot lambi na ho jaye
                .Select(a => new AuditLogDto
                {
                    AuditLogId = a.AuditLogId,
                    UserName = a.User.FullName,
                    Action = a.Action,
                    EntityName = a.EntityName,
                    EntityId = a.EntityId,
                    OldValue = a.OldValue,
                    NewValue = a.NewValue,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }
    }
}