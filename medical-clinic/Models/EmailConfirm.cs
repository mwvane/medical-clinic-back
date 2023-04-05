using System.ComponentModel.DataAnnotations;

namespace medical_clinic.Models
{
    public class EmailConfirm
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public DateTime ValidDate { get; set; }
        
    }
}
