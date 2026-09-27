using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_Self_Service.Modals.Entities
{
    public class RoleActivities
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Role { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Screen { get; set; }
        [Required]
        public bool canView { get; set; }
        [Required]
        public bool canAdd { get; set; }
        [Required]
        public bool canEdit { get; set; }
        [Required]
        public bool canDelete { get; set; }
    }
}
