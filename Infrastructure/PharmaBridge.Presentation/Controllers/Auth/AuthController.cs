using PharmaBridge.Abstraction.IServices.Auth;
using PharmaBridge.Shared.Dto_s.Auth.ForgetPssword;
using PharmaBridge.Shared.Dto_s.Auth.Sign_In_Up;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

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
        public async Task<ActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Success(result, "Account Created Successfully");
        }

        // 2. (Login)
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Success(result, "Login Successfully");
        }

        // 3. (Forget Password)
        [HttpPost("forget-password")]
        public async Task<ActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {
            await _authService.ForgetPasswordAsync(dto);
            return Success("Send OTP to your email");
        }

        // 4. Check OTP
        [HttpPost("verify-otp")]
        public async Task<ActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var isValid = await _authService.VerifyOtpAsync(dto);

            if (!isValid)
                return BadRequestError("OTP Not Valid or Incorrect");

            return Success("OTP is Valid");
        }

        // 5. (Reset Password)
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            return Success(result, "your password has changed successfully");
        }
    }
}
