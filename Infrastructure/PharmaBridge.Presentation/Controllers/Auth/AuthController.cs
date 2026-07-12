using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PharmaBridge.Abstraction.IServices.Auth;
using PharmaBridge.Shared.Common.Response;
using PharmaBridge.Shared.Dto_s.Auth.ForgetPssword;
using PharmaBridge.Shared.Dto_s.Auth.Sign_In_Up;
using PharmaBridge.Shared.DTOs.Auth.Sign_In_Up;

namespace PharmaBridge.Presentation.Controllers.Auth
{
    public class AuthController : AppBaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // 1. (Register)
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthModelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Success(result, "Account Created Successfully");
        }

        // 2. (Login)
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthModelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Success(result, "Login Successfully");
        }

        // 3. (Forget Password)
        [HttpPost("forget-password")]
        [EnableRateLimiting("OtpPolicy")]
        public async Task<ActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {
            await _authService.ForgetPasswordAsync(dto);
            return Success("Send OTP to your email");
        }

        // 4. Check OTP
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var isValid = await _authService.VerifyOtpAsync(dto);

            if (!isValid)
                return BadRequestError("OTP Not Valid or Incorrect");

            return Success("OTP is Valid");
        }

        // 5. (Reset Password)
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            return Success(result, "your password has changed successfully");
        }

        // 6. (Google Smart Login / Register)
        [HttpPost("google-auth")]
        [ProducesResponseType(typeof(ApiResponse<AuthModelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GoogleAuth([FromBody] GoogleAuthDto dto)
        {
            var result = await _authService.GoogleAuthAsync(dto);
            return Success(result, "Authenticated with Google Successfully");
        }
    }
}