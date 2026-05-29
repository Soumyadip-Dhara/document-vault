using documentvaultapi.BAL.Services.Interfaces;
using documentvaultapi.BAL.Services;
using documentvaultapi.DAL;
using documentvaultapi.DAL.Repositories.Interfaces;
using documentvaultapi.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Minio;
using documentvaultapi.Filters;
using documentvaultapi.Extensions;
using documentvaultapi.RbbitMQ;
using documentvaultapi.RabbitMQ.IRepositories;
using documentvaultapi.RabbitMQ.Repositories;
using documentvaultapi.RbbitMQ.Extensions;
using documentvaultapi.RbbitMQ.Services.Interfaces;
using documentvaultapi.RbbitMQ.Services;

var builder = WebApplication.CreateBuilder(args);



// =======================
// DbContext (EF Core 8 + PostgreSQL)
// =======================
builder.Services.AddDbContext<DocumentVaultDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DocumentVaultDB")
    ));

// =======================
// RabbitMQ
// =======================
builder.Services
   .AddRabbitMQ(builder.Configuration)
   .AddMessageProcessing();


// =======================
// Controllers
// =======================

builder.Services.AddControllers();

// AddHttpContextAccessor
builder.Services.AddHttpContextAccessor();


// =======================
// Repositories
// =======================
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IBucketRepository, BucketRepository>();
builder.Services.AddScoped<IApplicationMapRepository, ApplicationMapRepository>();
//Message Queue
builder.Services.AddTransient<IMessageQueueRepository, MessageQueueRepository>();
builder.Services.AddTransient<IConsumeLogRepository, ConsumeLogRepository>();
builder.Services.AddTransient<IConsumeFailedLogRepository, ConsumeFailedLogRepository>();
builder.Services.AddTransient<IMessageQueueFailedLogsRepository, MessageQueueFailedLogsRepository>();
builder.Services.AddTransient<IPublishedAcknowledgementLogRepository, PublishedAcknowledgementLogRepository>();
builder.Services.AddTransient<IConsumedAcknowledgementLogRepository, ConsumedAcknowledgementLogRepository>();



// =======================
// Services
// =======================
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IBucketService, BucketService>();
//Queue
builder.Services.AddTransient<IRabbitMqService, RabbitMqService>();
builder.Services.AddTransient<IMQueueProcessingService, MQueueProcessingService>();
//builder.Services.AddTransient<IMQueueProcessingService, MQueueProcessingService>(); // TODO: Need for publish
// Configure Hangfire
builder.Services.AddHangfireServices(builder.Configuration);







// =======================
// Filters
// =======================
builder.Services.AddScoped<ApplicationAuthFilter>();


// =======================
// MinIO client  
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
// Automapper
// =======================
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


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
app.UseRouting();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
