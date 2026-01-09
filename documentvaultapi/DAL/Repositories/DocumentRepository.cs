using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace documentvaultapi.DAL.Repositories
{
    public class DocumentRepository
        : Repository<Documents, DocumentVaultDbContext>, IDocumentRepository
    {
        public DocumentRepository(DocumentVaultDbContext context)
            : base(context)
        {
        }

        public async Task<bool> ExistsAsync(string bucketName, string objectName)
        {
            return await DocumentVaultDbContext.Documents.AnyAsync(d =>
                d.BucketName == bucketName &&
                d.ObjectName == objectName &&
                d.IsActive);
        }

        public async Task<Documents> InsertAsync(Documents document)
        {
            Add(document);          // from base repository
            SaveChangesManaged();   // from base repository
            return document;
        }

        public async Task<Documents?> GetByIdAsync(Guid documentId)
        {
            return await DocumentVaultDbContext.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(d =>
                    d.Id == documentId &&
                    d.IsActive == true);
        }

        public async Task<Documents?> GetSingleAsync(Expression<Func<Documents, bool>> predicate)
        {
            return await DocumentVaultDbContext.Set<Documents>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> MarkInactiveAsync(Guid id)
        {
            var entity = await DocumentVaultDbContext.Set<Documents>().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Metadata not found for document id: " + id);

            entity.IsActive = false;
            await DocumentVaultDbContext.SaveChangesAsync();

            return true;
        }

    }
}
