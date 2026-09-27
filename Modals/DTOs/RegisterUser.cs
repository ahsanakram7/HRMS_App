using System.ComponentModel.DataAnnotations;

namespace Employee_Self_Service.Modals.DTOs
{
    public class RegisterUser
    {
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
