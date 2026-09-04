using System;

namespace InventorySaaS_Application.Domain.Entities
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; } = string.Empty;       // Created / Updated / StatusChanged / GoodsReceived...
        public string EntityName { get; set; } = string.Empty;   // e.g. "PurchaseOrder", "Payment"
        public Guid EntityId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IPAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}