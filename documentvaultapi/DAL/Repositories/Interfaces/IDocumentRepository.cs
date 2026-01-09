// Interface for document repository
using documentvaultapi.DAL.Entities;
using System.Linq.Expressions;

namespace documentvaultapi.DAL.Repositories.Interfaces
{
    public interface IDocumentRepository : IRepository<Documents>
    {
        Task<Documents> InsertAsync(Documents document);
        Task<bool> ExistsAsync(string bucketName, string objectName);
        Task<Documents?> GetByIdAsync(Guid documentId);
        Task<Documents?> GetSingleAsync(Expression<Func<Documents, bool>> predicate);
        Task<bool> MarkInactiveAsync(Guid id);
    }
}
