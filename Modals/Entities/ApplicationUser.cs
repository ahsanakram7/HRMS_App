using Microsoft.AspNetCore.Identity;

namespace Employee_Self_Service.Modals.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
