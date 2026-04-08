using PharmaBridge.Shared.Dto_s.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.Token
{
    // This interface defines the contract for the token service
    // which will be responsible for generating and validating tokens for authentication and authorization purposes.
    // used in Auth Service to generate tokens for the users and providers after they log in or register,
    // and it will be used in the middleware to validate the tokens in the incoming requests.
    public interface ITokenService
    {
        Task<TokenResponseDto> CreateTokenAsync(TokenRequestDto tokenRequest);

    }
}
