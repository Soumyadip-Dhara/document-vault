//using documentvaultapi.DAL.Repositories.Interfaces;
//using documentvaultapi.Enum;
//using documentvaultapi.Helper;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace documentvaultapi.Filters
//{
//    public class ApplicationAuthFilter : IAsyncActionFilter
//    {
//        private readonly IApplicationMapRepository _repository;

//        public ApplicationAuthFilter(IApplicationMapRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task OnActionExecutionAsync(
//            ActionExecutingContext context,
//            ActionExecutionDelegate next)
//        {
//            var headers = context.HttpContext.Request.Headers;

//            // -----------------------
//            // Header existence check
//            // -----------------------
//            if (!headers.TryGetValue("app_id", out var appIdValue) ||
//                !headers.TryGetValue("client_secret", out var clientSecretValue))
//            {
//                context.Result = new BadRequestObjectResult(
//                    new APIResponseClass<string>
//                    {
//                        apiResponseStatus = APIResponseStatus.Error,
//                        message = "Missing app_id or client_secret"
//                    });
//                return;
//            }

//            // -----------------------
//            // Parse values
//            // -----------------------
//            if (!long.TryParse(appIdValue, out var appId) ||
//                !Guid.TryParse(clientSecretValue, out var clientSecret))
//            {
//                context.Result = new BadRequestObjectResult(
//                    new APIResponseClass<string>
//                    {
//                        apiResponseStatus = APIResponseStatus.Error,
//                        message = "Invalid app_id or client_secret format"
//                    });
//                return;
//            }

//            // -----------------------
//            // Validate against DB
//            // -----------------------
//            var valid = await _repository.ValidateAsync(appId, clientSecret);

//            if (valid == null)
//            {
//                context.Result = new UnauthorizedObjectResult(
//                    new APIResponseClass<string>
//                    {
//                        apiResponseStatus = APIResponseStatus.Error,
//                        message = "Invalid or inactive application credentials"
//                    });
//                return;
//            }

//            // -----------------------
//            // Proceed to controller
//            // -----------------------
//            await next();
//        }
//    }
//}

using documentvaultapi.DAL.Repositories.Interfaces;
using documentvaultapi.Enum;
using documentvaultapi.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace documentvaultapi.Filters
{
    public class ApplicationAuthFilter : IAsyncActionFilter
    {
        private readonly IApplicationMapRepository _repository;

        public ApplicationAuthFilter(IApplicationMapRepository repository)
        {
            _repository = repository;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var headers = context.HttpContext.Request.Headers;

            // ================================
            // 1. Check header existence
            // ================================
            if (!headers.TryGetValue("app_id", out var appIdValue) ||
                !headers.TryGetValue("client_secret", out var clientSecretValue))
            {
                context.Result = new UnauthorizedObjectResult(
                    new APIResponseClass<string>
                    {
                        apiResponseStatus = APIResponseStatus.Error,
                        message = "Missing app_id or client_secret in request headers"
                    });

                return;
            }

            // ================================
            // 2. Parse values
            // ================================
            if (!long.TryParse(appIdValue.ToString(), out var appId) ||
                !Guid.TryParse(clientSecretValue.ToString(), out var clientSecret))
            {
                context.Result = new UnauthorizedObjectResult(
                    new APIResponseClass<string>
                    {
                        apiResponseStatus = APIResponseStatus.Error,
                        message = "Invalid app_id or client_secret format"
                    });

                return;
            }

            // ================================
            // 3. Extract caller base URL
            // ================================
            var request = context.HttpContext.Request;

            var callerBaseUrl =
                $"{request.Scheme}://{request.Host}";

            // Normalize to avoid trailing slash issues
            callerBaseUrl = callerBaseUrl.TrimEnd('/');

            // ================================
            // 4. Validate base URL mapping via repository
            // ================================
            var validApp = await _repository
                .ValidateAsync(appId, clientSecret, callerBaseUrl);

            if (validApp == null)
            {
                context.Result = new UnauthorizedObjectResult(
                    new APIResponseClass<string>
                    {
                        apiResponseStatus = APIResponseStatus.Error,
                        message = "Application credentials not mapped with this base_url or inactive"
                    });

                return;
            }

            // ================================
            // 5. Proceed to action
            // ================================
            await next();
        }
    }
}
