// Interface for bucket service
using documentvaultapi.DAL.DTOs;

namespace documentvaultapi.BAL.Services.Interfaces
{
    public interface IBucketService
    {
        Task CreateBucketAsync(string bucketName);
        Task DeleteBucketAsync(string bucketName);
    }
}
