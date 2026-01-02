// Interface for document repository
using documentvaultapi.DAL.Entities;

namespace documentvaultapi.DAL.Repositories.Interfaces
{
    public interface IDocumentRepository : IRepository<documents>
    {
        Task<documents> InsertAsync(documents document);
        Task<bool> ExistsAsync(string bucketName, string objectName);
        Task<documents?> GetByIdAsync(Guid documentId);
    }
}
