namespace PharmaBridge.Shared.DTOs.Admin.Dashboard
{
    public class AdminActivityDto
    {
        public DateTime ActivityAt { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
    }
}