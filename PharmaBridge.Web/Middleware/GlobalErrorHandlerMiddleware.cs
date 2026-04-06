using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Shared.Common.Response;
using System.Text.Json;

namespace PharmaBridge.Web.Middleware
{
    public class GlobalErrorHandlerMiddleware
        (RequestDelegate nextMW , ILogger<GlobalErrorHandlerMiddleware> logger , IWebHostEnvironment env)
    {
        // 1 - Invoke the next middleware in the pipeline and catch any exceptions that occur
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await nextMW(context);
                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    // function handel 404 not found
                    await HandelResponseException(context, 404, "The requested resource was not found.");
                }
                // function handel response 
            }
            catch (Exception e) 
            {
                logger.LogError(e, $"Error : {e.Message}");
                // function handel exception 
                await HadelExceptionAsync(context, e);

            }
        }

        public async Task HandelResponseException(HttpContext context, int statusCode, string msg)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<string>(msg, statusCode);

            var jsonResponse = JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
             );
            await context.Response.WriteAsync(jsonResponse);
        }

        public async Task HadelExceptionAsync(HttpContext context, Exception ex) 
        {
            context.Response.ContentType = "application/json";
            string serverErrorMsg = env.IsDevelopment() ? $"{ex.Message} \n {ex.StackTrace}" 
                : "An unexpected error occurred. Please try again later.";

            var reponse = ex switch
            {
                NotFoundCutomeException => new ApiResponse<string>(ex.Message, 404),
                UnAuthorizedCustomeException => new ApiResponse<string>(ex.Message, 401),
                BadRequestCustomeException BR => new ApiResponse<string>(ex.Message, 400, errors: BR.Errors?.ToList()),
                _ => new ApiResponse<string>(serverErrorMsg, 500)
            };

            context.Response.StatusCode = reponse.StatusCode;
            reponse.IsSuccess = false;

            var jsonResponse = JsonSerializer.Serialize(
                reponse,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
             );
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
