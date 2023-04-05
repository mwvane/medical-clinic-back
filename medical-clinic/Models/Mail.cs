namespace medical_clinic.Models
{
    public class Mail
    {
        public List<string> EmailTo { get; set; } = new List<string>();
        public string EmailFromId { get; set; } = "bzishvili57@gmail.com";
        public string EmailFromPassword { get; set; } = "Ok Googlee";
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public bool IsBodyHtml { get; set; } = true;
        public List<string>? Attachments { get; set; } = new List<string>();
    }
}
