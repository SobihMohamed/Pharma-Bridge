using PharmaBridge.Shared.Dto_s.Auth.ForgetPssword;
using PharmaBridge.Shared.Dto_s.Auth.Sign_In_Up;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.Auth
{
        // This interface defines the contract for the authentication service
        // which will be responsible for handling user authentication, registration, password management, and OTP verification.
        public interface IAuthService
        {
            Task<AuthModelDto> LoginAsync(LoginDto loginDto);
            Task<AuthModelDto> RegisterAsync(RegisterDto registerDto);
            Task ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto); // return otp to reset password
            Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
            Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto); // to still login after reset password
        }
}
