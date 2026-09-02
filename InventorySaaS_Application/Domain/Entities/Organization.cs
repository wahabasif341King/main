using System;
using System.Collections.Generic;

namespace InventorySaaS_Application.Domain.Entities
{
    public class Organization
    {
        public Guid OrganizationId { get; set; } = Guid.NewGuid();
        
        public string Name { get; set; } = string.Empty;
        
        public string Currency { get; set; } = "PKR";
        
        public string Status { get; set; } = "Active";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}