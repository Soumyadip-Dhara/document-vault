using documentvaultapi.BAL.Services.Interfaces;
using documentvaultapi.DAL.DTOs;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories;
using documentvaultapi.DAL.Repositories.Interfaces;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;


namespace documentvaultapi.BAL.Services
{
    public class BucketService : IBucketService
    {
        private readonly IBucketRepository _bucketRepository;
        private readonly IMinioClient _minioClient;
        private readonly IConfiguration _configuration;

        public BucketService(
            IBucketRepository bucketRepository,
            IMinioClient minioClient,
            IConfiguration configuration)
        {
            _bucketRepository = bucketRepository;
            _minioClient = minioClient;
            _configuration = configuration;
        }

        public BucketService(IBucketRepository bucketRepository)
        {
            _bucketRepository = bucketRepository;
        }

        public async Task CreateBucketAsync(string bucketName)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new Exception("Bucket name cannot be empty");

            var exists = await _bucketRepository.BucketExistsAsync(bucketName);
            if (exists)
                throw new Exception("Bucket already exists");

            await _bucketRepository.CreateBucketAsync(bucketName);
        }

        public async Task DeleteBucketAsync(string bucketName)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new Exception("Bucket name cannot be empty");

            var exists = await _bucketRepository.BucketExistsAsync(bucketName);
            if (!exists)
                throw new Exception("Bucket does not exist");

            // ⚠️ Bucket must be empty
            await _bucketRepository.DeleteBucketAsync(bucketName);
        }

    }
}
