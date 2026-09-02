

namespace InventorySaaS_Application.Application.DTOs
{
    // Business signup: naya Organization (tenant) + uska Company Admin banata hai
    public class RegisterOrganizationDto
    {
        public string OrganizationName { get; set; } = string.Empty;
        
        public string AdminFullName { get; set; } = string.Empty;
        
        public string AdminEmail { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
        
        public string? Currency { get; set; } = "PKR";
    }
}