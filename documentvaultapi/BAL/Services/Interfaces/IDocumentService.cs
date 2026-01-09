// Interface for document service
using documentvaultapi.DAL.DTOs;

namespace documentvaultapi.BAL.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentUploadResponseDTO> UploadAsync(
            IFormFile file,
            long? createdBy);
        Task<(Stream Stream, string ContentType, string FileName)>DownloadAsync(Guid documentId);
        //Task<DocumentDownloadResponseDTO> GetDownloadUrlAsync(Guid documentId);

        Task<bool> DeleteDocumentAsync(Guid documentId);
    }
}
