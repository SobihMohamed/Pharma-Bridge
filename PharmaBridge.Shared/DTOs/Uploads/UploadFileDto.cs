using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace PharmaBridge.Shared.Dto_s.Attachment
{
    public class UploadFileDto
    {
        public IFormFile File { get; set; } = null!;

        // wwwroot ( "Images/Profiles" or "Documents/CVs")
        public string FolderName { get; set; } = null!;
    }
}