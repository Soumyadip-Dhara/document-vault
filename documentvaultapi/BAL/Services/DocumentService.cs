using documentvaultapi.BAL.Services.Interfaces;
using documentvaultapi.DAL.DTOs;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;


namespace documentvaultapi.BAL.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMinioClient _minioClient;
        private readonly IConfiguration _configuration;

        public DocumentService(
            IDocumentRepository documentRepository,
            IMinioClient minioClient,
            IConfiguration configuration)
        {
            _documentRepository = documentRepository;
            _minioClient = minioClient;
            _configuration = configuration;
        }

        public async Task<DocumentUploadResponseDTO> UploadAsync(
    IFormFile file,
    long? createdBy)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            var bucketName = _configuration["Minio:Bucket"]!;
            var objectName = $"{Guid.NewGuid()}_{file.FileName}";

            if (!await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucketName)))
            {
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(bucketName));
            }

            using var stream = file.OpenReadStream();

            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length)
                    .WithContentType(file.ContentType)
            );

            var entity = new documents
            {
                id = Guid.NewGuid(),
                bucket_name = bucketName,
                object_name = objectName,
                original_file_name = file.FileName,
                content_type = file.ContentType,
                file_size = file.Length,
                created_by = createdBy,
                is_active = true
            };

            var saved = await _documentRepository.InsertAsync(entity);

            // ✅ Correct DTO mapping
            return new DocumentUploadResponseDTO
            {
                DocumentId = saved.id,
                FileName = saved.original_file_name,
                Status = "Uploaded"
            };
        }
        public async Task<(Stream Stream, string ContentType, string FileName)>
    DownloadAsync(Guid documentId)
        {
            var doc = await _documentRepository.GetSingleAysnc(d => d.id == documentId && d.is_active);
            if (doc == null)
                throw new Exception("Document not found");

            var ms = new MemoryStream();

            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(doc.bucket_name)
                    .WithObject(doc.object_name)
                    .WithCallbackStream(stream =>
                    {
                        stream.CopyTo(ms);
                    })
            );

            ms.Position = 0;

            return (ms, doc.content_type ?? "application/octet-stream", doc.original_file_name);
        }
        //public async Task<DocumentDownloadResponseDTO> GetDownloadUrlAsync(Guid documentId)
        //{
        //    // -------------------
        //    // Fetch metadata
        //    // -------------------
        //    var document = await _repository.GetByIdAsync(documentId);

        //    if (document == null)
        //        throw new Exception("Document not found");

        //    // -------------------
        //    // Generate pre-signed URL
        //    // -------------------
        //    int expirySeconds = 15 * 60; // 15 minutes

        //    var presignedUrl = await _minioClient.PresignedGetObjectAsync(
        //        new PresignedGetObjectArgs()
        //            .WithBucket(document.bucket_name)
        //            .WithObject(document.object_name)
        //            .WithExpiry(expirySeconds)
        //    );

        //    // 🔁 Replace internal MinIO hostname with public hostname
        //    var internalEndpoint = _configuration["Minio:Endpoint"];
        //    var publicEndpoint = _configuration["Minio:PublicEndpoint"];

        //    presignedUrl = presignedUrl.Replace(internalEndpoint, publicEndpoint);

        //    return new DocumentDownloadResponseDTO
        //    {
        //        DocumentId = document.id,
        //        FileName = document.original_file_name,
        //        DownloadUrl = presignedUrl,
        //        ExpiryAt = DateTime.UtcNow.AddSeconds(expirySeconds)
        //    };
        //}

    }
}
