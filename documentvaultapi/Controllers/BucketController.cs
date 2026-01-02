using documentvaultapi.BAL.Services.Interfaces;
using documentvaultapi.DAL.DTOs;
using documentvaultapi.Helper;
using documentvaultapi.Enum;
using Microsoft.AspNetCore.Mvc;

namespace documentvaultapi.Controllers
{
    [ApiController]
    [Route("api/Documents")]
    public class BucketController : ControllerBase
    {
        private readonly IBucketService _bucketService;

        public BucketController(IBucketService bucketService)
        {
            _bucketService = bucketService;
        }

        // ============================
        // Create Bucket
        // ============================
        [HttpPost("{bucketName}")]
        public async Task<APIResponseClass<string>> CreateBucket(string bucketName)
        {
            APIResponseClass<string> response = new();

            try
            {
                await _bucketService.CreateBucketAsync(bucketName);

                response.apiResponseStatus = APIResponseStatus.Success;
                response.message = "Bucket created successfully";
                response.result = bucketName;
            }
            catch (Exception ex)
            {
                response.apiResponseStatus = APIResponseStatus.Error;
                response.message = ex.Message;
            }

            return response;
        }

        // ============================
        // Delete Bucket
        // ============================
        [HttpDelete("{bucketName}")]
        public async Task<APIResponseClass<string>> DeleteBucket(string bucketName)
        {
            APIResponseClass<string> response = new();

            try
            {
                await _bucketService.DeleteBucketAsync(bucketName);

                response.apiResponseStatus = APIResponseStatus.Success;
                response.message = "Bucket deleted successfully";
                response.result = bucketName;
            }
            catch (Exception ex)
            {
                response.apiResponseStatus = APIResponseStatus.Error;
                response.message = ex.Message;
            }

            return response;
        }
    }
}
