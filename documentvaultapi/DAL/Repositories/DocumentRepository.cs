using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace documentvaultapi.DAL.Repositories
{
    public class DocumentRepository
        : Repository<documents, DocumentVaultDbContext>, IDocumentRepository
    {
        public DocumentRepository(DocumentVaultDbContext context)
            : base(context)
        {
        }

        public async Task<bool> ExistsAsync(string bucketName, string objectName)
        {
            return await UMDbContext.documents.AnyAsync(d =>
                d.bucket_name == bucketName &&
                d.object_name == objectName &&
                d.is_active);
        }

        public async Task<documents> InsertAsync(documents document)
        {
            Add(document);          // from base repository
            SaveChangesManaged();   // from base repository
            return document;
        }

        public async Task<documents?> GetByIdAsync(Guid documentId)
        {
            return await UMDbContext.documents
                .AsNoTracking()
                .FirstOrDefaultAsync(d =>
                    d.id == documentId &&
                    d.is_active == true);
        }

    }
}
