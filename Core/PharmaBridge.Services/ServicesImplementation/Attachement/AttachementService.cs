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
            {
                throw new ArgumentNullException(nameof(uploadFileDto.File), "File cannot be null or empty");
            }

            // 1 - check allowed extensions
            var fileExtension = Path.GetExtension(uploadFileDto.File.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !_allowedExtensions.Contains(fileExtension))
                throw new BadRequestCustomeException("File type is not allowed. Allowed types are: jpg, png, jpeg, pdf");

            // 2 - check file size
            var fileSize = uploadFileDto.File.Length;
            if (fileSize > _fileSizeLimit)
                throw new BadRequestCustomeException("This file is too large. Max size is 8MB.");

            // 3 - Get Root Path (wwwroot) + SubFolder
            var webRootPath = webHostEnvironment.WebRootPath;

            // if wwwroot path is null,
            // we can set it to a default value (like "wwwroot" in the current directory)
            // to avoid errors when trying to save files
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var folderPath = Path.Combine(webRootPath, uploadFileDto.FolderName);

            // 4 - check if folderPath Exist
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // 5 - create unique file name
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // 6 - create Full Path 
            var fullPath = Path.Combine(folderPath, uniqueFileName);

            // 7 - Save inside HARDDISK
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await uploadFileDto.File.CopyToAsync(stream);
            }

            // return the relative path to the file
            // (to be stored in the database or used for accessing the file later)
            return Path.Combine(uploadFileDto.FolderName, uniqueFileName).Replace("\\", "/");
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
