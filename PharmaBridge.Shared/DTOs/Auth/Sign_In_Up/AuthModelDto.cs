using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Dto_s.Auth.Sign_In_Up
{
    public class AuthModelDto
    {
        public string Message { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public string phoneNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireOn { get; set; }
        public ICollection<string> Roles { get; set; } = new HashSet<string>();

    }
}
