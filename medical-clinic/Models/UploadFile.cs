﻿namespace medical_clinic.Models
{
    public class UploadFile
    {
        public int UserId { get; set; }
        public IFormFile File { get; set; }
    }
}
