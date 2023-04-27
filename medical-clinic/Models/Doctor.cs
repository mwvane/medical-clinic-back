using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace medical_clinic.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        [AllowNull]
        public int? Views { get; set; }
        [AllowNull]
        public int? Rating { get; set; }
        
    }
}
