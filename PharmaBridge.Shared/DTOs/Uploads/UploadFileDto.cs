using System;
using System.IO;

namespace PharmaBridge.Shared.Dto_s.Attachment
{
    public class UploadFileDto
    {
        public Stream Content { get; set; }
        public string FileName { get; set; }
        public string FolderName { get; set; }
        public string ContentType { get; set; }
    }
}