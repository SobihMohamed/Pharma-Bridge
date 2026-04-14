using PharmaBridge.Shared.Dto_s.Attachment;

namespace PharmaBridge.Abstraction.IServices.Attachement
{
    // It will help any other service that needs to handle the attachments to do it in a unified way.
    // It will be used by the provider and client profiles services to handle the profile picture and the cover picture,
    // and it can be used by any other service that needs to handle the attachments of the providers and clients.
    public interface IAttachementService
    {
        // Uploads a file using the provided DTO and returns the file path/URL
        Task<string> UploadFileAsync(UploadFileDto uploadFileDto);

        // Deletes an existing file from the storage
        Task<bool> DeleteFileAsync(string filePath);
    }
}