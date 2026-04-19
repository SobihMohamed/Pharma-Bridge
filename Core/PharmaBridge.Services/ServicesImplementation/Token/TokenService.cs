using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PharmaBridge.Abstraction.IServices.Token;
using PharmaBridge.Shared.Dto_s.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SoftBridge.Services.Services.Token
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        public Task<TokenResponseDto> CreateTokenAsync(TokenRequestDto tokenRequest)
        {
            // 1 - get data from tokenRequest
            var userId = tokenRequest.UserId;
            var userName = tokenRequest.UserName;
            var email = tokenRequest.Email;
            var roles = tokenRequest.Roles;

            // 2 - create claims based on the data
            // used to store information about the user and their roles in the token
            // which can be used for authentication and authorization purposes when user login to system 
            // from token we get the data of him and his roles and
            // we can use it to determine what actions he can perform in the system
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email)
            };
            // 3 - loop through the roles and add them as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 4 - get the secret key from configuration and create a symmetric security key 
            // we get it as string 
            var secretKey = configuration.GetSection("JwtTokenSettings:SecretKey").Value;

            // 5 - create signing credentials using the symmetric security key and a hashing algorithm
            // convert secret key to byte array using UTF8 encoding
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            // use signing credentials to sign the token using HMAC SHA256 algorithm
            // to produce the final token that can be used for authentication and authorization in the system
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 6 - create expire date for the token
            var expireTime = DateTime.UtcNow.AddHours(1);

            // 7 - create a security token descriptor that contains the claims, signing credentials, and expire date
            // install the package System.IdentityModel.Tokens.Jwt to use the class SecurityTokenDescriptor
            var TokenDesc = new JwtSecurityToken(
                issuer: configuration.GetSection("JwtTokenSettings:Issuer").Value, // the entity that issues the token, typically the authentication server or service that generates the token
                audience: configuration.GetSection("JwtTokenSettings:Audience").Value, // the intended recipient of the token, which is usually the resource server or API that will consume the token for authentication and authorization purposes
                claims: claims,
                expires: expireTime,
                signingCredentials: signingCredentials
            );

            // 8 - create a token handler and write the token to a string format that can be returned to the client
            var token = new JwtSecurityTokenHandler().WriteToken(TokenDesc);

            var tokenResponse = new TokenResponseDto
            {
                Token = token,
                ExpireOn = expireTime
            };

            return Task.FromResult(tokenResponse);
        }
    }
}
