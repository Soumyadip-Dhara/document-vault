using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;

namespace documentvaultapi.DAL.Repositories
{
    public class BucketRepository
        : Repository<documents, DocumentVaultDbContext>, IBucketRepository

    {
        private readonly IMinioClient _minioClient;
        public BucketRepository(DocumentVaultDbContext context, IMinioClient minioClient)
            : base(context)
        {
            _minioClient = minioClient;
        }

        public async Task<bool> BucketExistsAsync(string bucketName)
        {
            return await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucketName)
            );
        }

        public async Task CreateBucketAsync(string bucketName)
        {
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucketName)
            );
        }

        public async Task DeleteBucketAsync(string bucketName)
        {
            await _minioClient.RemoveBucketAsync(
                new RemoveBucketArgs().WithBucket(bucketName)
            );
        }

    }
}
