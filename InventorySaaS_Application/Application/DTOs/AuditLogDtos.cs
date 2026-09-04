using System;

namespace InventorySaaS_Application.Application.DTOs
{
    public class AuditLogDto
    {
        public Guid AuditLogId { get; set; }
        public string UserName { get; set; } = default!;
        public string Action { get; set; } = default!;
        public string EntityName { get; set; } = default!;
        public Guid EntityId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}