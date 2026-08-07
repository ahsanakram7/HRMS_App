using System.ComponentModel.DataAnnotations;

namespace Employee_Self_Service.Modals
{
    public class LoginUser
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
