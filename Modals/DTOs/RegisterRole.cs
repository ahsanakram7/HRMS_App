using System.ComponentModel.DataAnnotations;

namespace Employee_Self_Service.Modals.DTOs
{
    public class RegisterRole
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
