using Microsoft.AspNetCore.Hosting;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Shared.Dto_s.Attachment;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.ServicesImplementation.Attachement
{
    public class AttachmentService(IWebHostEnvironment webHostEnvironment) : IAttachementService
    {

        // to ensure that only specific file types are allowed and to prevent users from uploading potentially harmful files
        private readonly List<string> _allowedExtensions = new() { ".jpg", ".png", ".jpeg", ".pdf" };
        private const long _fileSizeLimit = 8 * 1024 * 1024; // 8 MB



        public async Task<string> UploadFileAsync(UploadFileDto uploadFileDto)
        {
            if (uploadFileDto.File == null || uploadFileDto.File.Length == 0)
                throw new ArgumentNullException(nameof(uploadFileDto.File), "File cannot be null or empty");

            // 1 - check allowed extensions
            var fileExtension = Path.GetExtension(uploadFileDto.File.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !_allowedExtensions.Contains(fileExtension))
                throw new BadRequestCustomeException("File type is not allowed. Allowed types are: jpg, png, jpeg, pdf");

            // 2 - check file size
            if (uploadFileDto.File.Length > _fileSizeLimit)
                throw new BadRequestCustomeException("This file is too large. Max size is 8MB.");

            // 3 - Get Root Path + Build per-user folder ✅ ONLY THIS CHANGED
            var webRootPath = webHostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // ✅ If UserId provided → requests/{userId}/
            // ✅ If no UserId       → original FolderName (fallback, nothing breaks)
            var relativeFolderPath = !string.IsNullOrEmpty(uploadFileDto.UserId)
                ? Path.Combine(uploadFileDto.FolderName, uploadFileDto.UserId)
                : uploadFileDto.FolderName;

            var folderPath = Path.Combine(webRootPath, relativeFolderPath);

            // 4 - check if folderPath Exist
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // 5 - create unique file name
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // 6 - create Full Path
            var fullPath = Path.Combine(folderPath, uniqueFileName);

            // 7 - Save to disk
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await uploadFileDto.File.CopyToAsync(stream);
            }

            // ✅ Returns: requests/{userId}/guid.jpg
            return Path.Combine(relativeFolderPath, uniqueFileName).Replace("\\", "/");
        }

        public Task<bool> DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return Task.FromResult(false);

            var webRootPath = webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.Combine(webRootPath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                // Task.FromResult(true) mean => the file was found and deleted successfully
                return Task.FromResult(true);
            }
            // Task.FromResult(false) mean => the file was not found or could not be deleted, but we don't want to throw an exception in this case
            return Task.FromResult(false);
        }
    }
}
