using documentvaultapi.BAL.Services.Interfaces;
using documentvaultapi.DAL.DTOs;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories;
using documentvaultapi.DAL.Repositories.Interfaces;
using documentvaultapi.Helper;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using System;
using System.Security.Cryptography;


namespace documentvaultapi.BAL.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMinioClient _minioClient;
        private readonly IConfiguration _configuration;

        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
{
    // PDFs
    "application/pdf",

    // Images
    "image/jpeg",
    "image/png",

    // Text
    "text/plain",

    // Word
    "application/msword",
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

    // Excel
    "application/vnd.ms-excel",
    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
};

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
{
    ".pdf",
    ".jpg",
    ".jpeg",
    ".png",
    ".txt",
    ".doc",
    ".docx",
    ".xls",
    ".xlsx"
};


        public DocumentService(
            IDocumentRepository documentRepository,
            IMinioClient minioClient,
            IConfiguration configuration)
        {
            _documentRepository = documentRepository;
            _minioClient = minioClient;
            _configuration = configuration;
        }

        private static string ComputeSha256Hash(Stream stream)
        {
            using var sha256 = SHA256.Create();

            var hashBytes = sha256.ComputeHash(stream);
            var hashString = BitConverter.ToString(hashBytes)
                .Replace("-", "")
                .ToLowerInvariant();

            return hashString;
        }

        public async Task<DocumentUploadResponseDTO> UploadAsync(
    IFormFile file,
    long? createdBy)
        {
            // -----------------------
            // File Null validation
            // -----------------------
            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            // -----------------------
            // File type validation
            // -----------------------
            var extension = Path.GetExtension(file.FileName);

            if (!AllowedExtensions.Contains(extension))
                throw new Exception("Invalid file type. Only PDF, images(jpg, jpeg, png), text, Word, and Excel files are allowed.");

            if (string.IsNullOrWhiteSpace(file.ContentType) ||
                !AllowedContentTypes.Contains(file.ContentType))
                throw new Exception("Invalid content type.");

            // -----------------------
            // (Optional) File size limit
            // -----------------------
            var maxFileSizeMb = _configuration.GetValue<int>("DocumentUpload:MaxFileSizeMB");
            var maxFileSizeBytes = maxFileSizeMb * 1024L * 1024L;
            if (file.Length > maxFileSizeBytes)
                throw new Exception($"File size exceeds {maxFileSizeMb} MB limit.");



            // -----------------------
            // Compute file hash
            // -----------------------
            string fileHash;

            using (var hashStream = file.OpenReadStream())
            {
                fileHash = ComputeSha256Hash(hashStream);
            }
            // -----------------------
            //  DUPLICATE CHECK 
            // -----------------------
            var existing = await _documentRepository
    .GetSingleAsync(d => d.FileHash == fileHash && d.IsActive);

            if (existing != null)
            {
                throw new DuplicateDocumentException(existing.Id);
            }



            // -----------------------
            // Upload to MinIO
            // -----------------------
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

            var entity = new Documents
            {
                Id = Guid.NewGuid(),
                BucketName = bucketName,
                ObjectName = objectName,
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                //application_id = applicationId,     TO DO LATER
                CreatedBy = createdBy,
                FileHash = fileHash,          
                IsActive = true
                
            };

            var saved = await _documentRepository.InsertAsync(entity);

            //  DTO mapping
            return new DocumentUploadResponseDTO
            {
                DocumentId = saved.Id,
                FileName = saved.OriginalFileName,
                Hash = saved.FileHash,
                Status = "Uploaded"
            };
        }
        public async Task<(Stream Stream, string ContentType, string FileName)>
    DownloadAsync(Guid documentId)
        {
            var doc = await _documentRepository.GetSingleAysnc(d => d.Id == documentId && d.IsActive);
            if (doc == null)
                throw new Exception("Document not found");

            var ms = new MemoryStream();

            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(doc.BucketName)
                    .WithObject(doc.ObjectName)
                    .WithCallbackStream(stream =>
                    {
                        stream.CopyTo(ms);
                    })
            );

            ms.Position = 0;

            return (ms, doc.ContentType ?? "application/octet-stream", doc.OriginalFileName);
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

        public async Task<bool> DeleteDocumentAsync(Guid documentId)
        {
            // --------------------------
            // 1. Read metadata
            // --------------------------
            var metadata = await _documentRepository.GetSingleAsync(x => x.Id == documentId && x.IsActive);

            if (metadata == null)
                throw new Exception("Document not found or already inactive: " + documentId);

            var bucket = metadata.BucketName;
            var objectName = metadata.ObjectName;

            // --------------------------
            // 2. Delete from MinIO
            // --------------------------
            //await _minioClient.RemoveObjectAsync(
            //    new RemoveObjectArgs()
            //        .WithBucket(bucket)
            //        .WithObject(objectName)
            //);

            // --------------------------
            // 3. Mark metadata inactive in DB
            // --------------------------
            await _documentRepository.MarkInactiveAsync(documentId);

            return true;
        }


    }
}
