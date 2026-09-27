using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_Self_Service.Modals.Entities
{
    public class Screens
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Route { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Icon { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
    }
}
