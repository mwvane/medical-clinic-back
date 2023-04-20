using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace medical_clinic.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string IdentityNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; } = "Client";
        [AllowNull]
        public string? ImageUrl { get; set; }
        [AllowNull]
        public string? Token { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
