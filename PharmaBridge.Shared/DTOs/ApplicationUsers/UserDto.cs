using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.ApplicationUsers
{
    // Changed name from UserDto to AuthResponseDto to reflect its actual purpose
    public class AuthResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireOn { get; set; }
        public ICollection<string> Roles { get; set; } = new HashSet<string>();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
    }
}
