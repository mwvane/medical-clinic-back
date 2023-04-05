﻿using System.ComponentModel.DataAnnotations;

namespace medical_clinic.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
