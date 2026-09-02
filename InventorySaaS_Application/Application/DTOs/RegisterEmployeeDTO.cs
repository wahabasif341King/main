

namespace InventorySaaS_Application.Application.DTOs
{
    // Same organization ke andar naya user add karta hai
    // RoleName: "Company Admin" | "Manager" | "Salesperson" | "Warehouse Staff" | "Accountant"
    public class RegisterEmployeeDto
    {
        public string FullName { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
        
        public string? PhoneNumber { get; set; }
        
        public string RoleName { get; set; } = string.Empty;
    }
}