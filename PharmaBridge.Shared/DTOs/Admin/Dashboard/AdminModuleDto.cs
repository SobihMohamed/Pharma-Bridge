namespace PharmaBridge.Shared.DTOs.Admin.Dashboard
{
    public class AdminModuleDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageId { get; set; } = string.Empty;
        public AdminModuleButtonDto PrimaryButton { get; set; } = new();
        public AdminModuleButtonDto? SecondaryButton { get; set; }
    }

    public class AdminModuleButtonDto
    {
        public string Label { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
    }
}