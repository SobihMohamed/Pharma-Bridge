using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace PharmaBridge.Shared.Dto_s.Token
{
    // This DTO is used to pass user information when creating a token.
    // It contains the user's ID, username, email, and roles.
    public class TokenRequestDto
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public ICollection<string> Roles { get; set; } = new HashSet<string>();
    }
}
