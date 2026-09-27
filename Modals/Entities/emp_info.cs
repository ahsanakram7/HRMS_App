using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_Self_Service.Modals.Entities
{
    public class emp_info
    {
        [Key]
        [Required]
        public int emp_no { get; set; }
        [Required]
        [MaxLength(50)]
        public string? full_name { get; set; }
        [MaxLength(1)]
        public string? part_full_time_flag { get; set; }
        [MaxLength(10)]
        public string? religion { get; set; }
        [MaxLength(10)]
        public string? emp_sex { get; set; }
        [MaxLength(3)]
        public string? blood_group { get; set; }
        [MaxLength(6)]
        public string? marital_status { get; set; }
        public DateTime? marital_dated { get; set; }
        public DateTime? date_of_birth { get; set; }
        [MaxLength(10)]
        public string? country_of_birth { get; set; }
        [MaxLength(15)]
        public string? city_of_birth { get; set; }
        [MaxLength(1)]
        public string? can_travel_local { get; set; }
        [MaxLength(1)]
        public string? can_travel_abroad { get; set; }
        [Phone]
        [MaxLength(15)]
        public string? mobile_no { get; set; }
        [EmailAddress]
        [MaxLength(15)]
        public string? official_email_add { get; set; }
        [EmailAddress]
        [MaxLength(15)]
        public string? personal_email_add { get; set; }
        
        [NotMapped]
        public int totalRecords { get; set; }
    }
}
