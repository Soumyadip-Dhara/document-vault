using documentvaultapi.BAL.Services.Interfaces;
using documentvaultapi.BAL.Services;
using documentvaultapi.DAL;
using documentvaultapi.DAL.Repositories.Interfaces;
using documentvaultapi.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// =======================
// DbContext (EF Core 8 + PostgreSQL)
// =======================
builder.Services.AddDbContext<DocumentVaultDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DocumentVaultDB")
    ));

// =======================
// Repositories
// =======================
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IBucketRepository, BucketRepository>();

// =======================
// Services
// =======================
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IBucketService, BucketService>();

// =======================
// MinIO client  ✅ ADD HERE
// =======================
builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    return new MinioClient()
        .WithEndpoint(config["Minio:Endpoint"])
        .WithCredentials(
            config["Minio:AccessKey"],
            config["Minio:SecretKey"])
        .Build();
});

// =======================
// Controllers
// =======================
builder.Services.AddControllers();

// =======================
// Swagger / OpenAPI
// =======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =======================
// Build app
// =======================
var app = builder.Build();

// =======================
// Middleware pipeline
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
