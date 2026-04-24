using PharmaBridge.Shared.Common.Response;
using Microsoft.AspNetCore.Mvc;

namespace PharmaBridge.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppBaseController : ControllerBase
    {
        // Standardized with data success response
        protected ActionResult Success<T>(T data, string message = "Operation Successfully")
        {
            return Ok(new ApiResponse<T>(data, message));
        }

        // Standardized without data success response
        protected ActionResult Success(string message = "Operation Successfully")
        {
            return Ok(new ApiResponse<string>(null!, message, 200));
        }

        // Standardized Created response
        protected ActionResult Created<T>(T data, string message = "Created Successfully")
        {
            return StatusCode(201, new ApiResponse<T>(data, message, 201));
        }

        protected ActionResult BadRequestError(string message)
        {
            return StatusCode(400, new ApiResponse<object>(null!, message, 400));
        }

        // 404 Not Found
        protected ActionResult NotFoundError(string message = "Resource not found")
        {
            return StatusCode(404, new ApiResponse<object>(null!, message, 404));
        }

        // 401 Unauthorized 
        protected ActionResult UnauthorizedError(string message = "You are not authorized")
        {
            return StatusCode(401, new ApiResponse<object>(null!, message, 401));
        }
    }
}
