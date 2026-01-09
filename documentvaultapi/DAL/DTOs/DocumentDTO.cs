// DTO for document upload response
namespace documentvaultapi.DAL.DTOs
{

    public class DocumentUploadRequestDTO
    {
        public IFormFile File { get; set; }
        public long? CreatedBy { get; set; }
    }

    public class DocumentUploadResponseDTO
    {
        public Guid DocumentId { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public string Hash { get; set; }
    }

    public class DocumentDownloadResponseDTO
    {
        public Guid DocumentId { get; set; }
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }
        public DateTime ExpiryAt { get; set; }
    }
}
