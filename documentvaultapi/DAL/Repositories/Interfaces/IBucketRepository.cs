// Interface for document repository
using documentvaultapi.DAL.Entities;

namespace documentvaultapi.DAL.Repositories.Interfaces
{
    public interface IBucketRepository : IRepository<Documents>
    {
        Task<bool> BucketExistsAsync(string bucketName);
        Task CreateBucketAsync(string bucketName);
        Task DeleteBucketAsync(string bucketName);
    }
}
