using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using PharmaBridge.Abstraction.IServices.Auth;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Abstraction.IServices.Token;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.Dto_s.Auth.ForgetPssword;
using PharmaBridge.Shared.Dto_s.Auth.Sign_In_Up;
using PharmaBridge.Shared.Dto_s.Token;
using PharmaBridge.Shared.DTOs.Auth.Sign_In_Up;
using PharmaBridge.Shared.DTOs.Notificaiton;
using PharmaBridge.Shared.EnumHelper.NotificationEnums;
using PharmaBridge.Shared.EnumHelper.UserEnums;

namespace PharmaBridge.Services.ServicesImplementation.Auth
{
    public class AuthService(UserManager<ApplicationUser> _userManager
        //INotificationService _notificationService
        , IMapper _mapper, ITokenService _tokenService , INotificationService _notificationService)
        : IAuthService
    {
        public async Task<AuthModelDto> RegisterAsync(RegisterDto registerDto)
        {
            //1 - Security Check: Prevent registering as Admin or sending invalid roles
            if (registerDto.Role == UserRole.Admin)
                throw new BadRequestCustomeException("Invalid Email or Password");

            if (!Enum.IsDefined(typeof(UserRole), registerDto.Role))
                throw new BadRequestCustomeException("Invalid user role selected.");

            // 2 - Check if the user already exists
            var user = await _userManager.FindByEmailAsync(registerDto.Email);
            if (user != null)
                throw new BadRequestCustomeException("User with this email already exists."); 

            // 3 - mapping the data from the DTO to the ApplicationUser model
            var userForDB = _mapper.Map<ApplicationUser>(registerDto);
            userForDB.Role = registerDto.Role;

            // 4 - Create the user in the database
            var result = await _userManager.CreateAsync(userForDB, registerDto.Password);

            // 5 - Check if the user creation was successful
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestCustomeException("User registration failed", errors);
            }

            // 6 - Assign the dynamic role to the user 
            string roleName = registerDto.Role.ToString();
            await _userManager.AddToRoleAsync(userForDB, roleName);

            var userRoles = await _userManager.GetRolesAsync(userForDB);

            // 7 - Generate Token
            var tokenRequest = new TokenRequestDto
            {
                UserId = userForDB.Id,
                Email = userForDB!.Email!,
                UserName = userForDB.FullName,
                Roles = userRoles 
            };

            var tokenResponse = await _tokenService.CreateTokenAsync(tokenRequest);
            if (string.IsNullOrEmpty(tokenResponse.Token))
                throw new BadRequestCustomeException("Token generation failed.");

            // 8 - Return the authentication model
            return new AuthModelDto
            {
                Token = tokenResponse.Token,
                IsAuthenticated = true,
                ExpireOn = tokenResponse.ExpireOn,
                Email = userForDB.Email!,
                Name = userForDB.FullName,
                phoneNumber = userForDB.PhoneNumber!,
                Roles = userRoles
            };
        }
        public async Task<AuthModelDto> LoginAsync(LoginDto loginDto)
        {

            // 1 - Get the user from the database
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            // 2 - Check if the user exists
            if (user == null)
                throw new UnAuthorizedCustomeException();

            // 3 - Check if the password is correct
            var result = await _userManager.CheckPasswordAsync(user!, loginDto.Password);
            if (!result)
                throw new UnAuthorizedCustomeException();

            // 4 - Generate Token
            var userRoles = await _userManager.GetRolesAsync(user!);
            var tokenRequest = new TokenRequestDto
            {
                UserId = user!.Id!,
                Email = user.Email!,
                UserName = user.FullName,
                Roles = userRoles
            };
            var tokenResp = await _tokenService.CreateTokenAsync(tokenRequest);
            if (string.IsNullOrEmpty(tokenResp.Token))
                throw new BadRequestCustomeException("Token generation failed.");

            // 5 - Return the authentication model with the token and user details
            return new AuthModelDto
            {
                Token = tokenResp.Token,
                IsAuthenticated = true,
                ExpireOn = tokenResp.ExpireOn,
                Email = user.Email!,
                Name = user.FullName,
                Roles = userRoles
            };
        }
        public async Task ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto)
        {
            // 1 - Get the user from the database
            var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
            if (user == null)
                return; // Security best practice: don't reveal if email exists or not

            // 2 - Generate OTP
            var otp = await _userManager.GeneratePasswordResetTokenAsync(user);

            // 3 - Send OTP via Notification Service
            var message = new NotificationContentDto
            {
                Email = user.Email!,
                Subject = "PharmaBridge - Password Reset OTP",
                Body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2>Password Reset Request</h2>
                <p>Hello {user.FullName},</p>
                <p>Your OTP code to reset your password is: <b style='font-size: 24px; color: #2563eb;'>{otp}</b></p>
                <p>This code is valid for a short period of time.</p>
                <p>If you didn't request this, please ignore this email.</p>
            </div>",
                UserId = user.Id,
                ReferenceId = null,
                Payload = null
            };

            await _notificationService.SendNotificationAsync(message, NotificationType.Email);
        }
        public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            // 1 - get the user from the database
            var user = await _userManager.FindByEmailAsync(verifyOtpDto.Email);
            if (user == null)
                return false;

            //2 - Verify the OTP
            var isvalid = await _userManager.VerifyUserTokenAsync(
                user,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                UserManager<ApplicationUser>.ResetPasswordTokenPurpose, // not _usermanager because the field is static 
                verifyOtpDto.Otp);

            return isvalid;
        }
        public async Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            // 1 - get the user from the database
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                throw new UnAuthorizedCustomeException();

            // 2 - change the user password by otp 
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Otp, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestCustomeException("Password reset failed.", errors);
            }

            // 3 - generate new token to kept the user login after change password 
            var userRoles = await _userManager.GetRolesAsync(user);
            var tokenRequest = new TokenRequestDto
            {
                UserId = user.Id,
                Email = user.Email!,
                UserName = user.FullName,
                Roles = userRoles,
            };
            var tokenRespo = await _tokenService.CreateTokenAsync(tokenRequest);

            return new AuthModelDto
            {
                Token = tokenRespo.Token,
                IsAuthenticated = true,
                ExpireOn = tokenRespo.ExpireOn,
                Email = user.Email!,
                Name = user.FullName,
                Roles = userRoles
            };
        }
        public async Task<AuthModelDto> GoogleAuthAsync(GoogleAuthDto googleAuthDto)
        {
            // 1 - Validate Google Token
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(googleAuthDto.IdToken);
            }
            catch (Exception)
            {
                throw new UnAuthorizedCustomeException("Invalid Google Token");
            }

            // 2 - Check if User Exists in our DB
            var user = await _userManager.FindByEmailAsync(payload.Email);

            // 3 - If User doesn't exist, Register him! (Smart Registration)
            if (user == null)
            {
                if (googleAuthDto.Role == null || !Enum.IsDefined(typeof(UserRole), googleAuthDto.Role))
                    throw new BadRequestCustomeException("User role is required for new Google registration.");

                if (googleAuthDto.Role == UserRole.Admin)
                    throw new BadRequestCustomeException("Cannot register as Admin.");

                user = new ApplicationUser
                {
                    UserName = payload.Email.Split('@')[0], 
                    Email = payload.Email,
                    FullName = payload.Name, 
                    EmailConfirmed = true, 
                    Role = googleAuthDto.Role.Value
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    throw new BadRequestCustomeException("Failed to create user from Google.");

                await _userManager.AddToRoleAsync(user, googleAuthDto.Role.ToString());
            }

            // 4 - Generate OUR System Token (Login part)
            var userRoles = await _userManager.GetRolesAsync(user);
            var tokenRequest = new TokenRequestDto
            {
                UserId = user.Id,
                Email = user.Email!,
                UserName = user.FullName,
                Roles = userRoles
            };

            var tokenResp = await _tokenService.CreateTokenAsync(tokenRequest);
            if (string.IsNullOrEmpty(tokenResp.Token))
                throw new BadRequestCustomeException("Token generation failed.");

            // 5 - Return standard AuthModelDto
            return new AuthModelDto
            {
                Token = tokenResp.Token,
                IsAuthenticated = true,
                ExpireOn = tokenResp.ExpireOn,
                Email = user.Email!,
                Name = user.FullName,
                Roles = userRoles
            };
        }
    }
}