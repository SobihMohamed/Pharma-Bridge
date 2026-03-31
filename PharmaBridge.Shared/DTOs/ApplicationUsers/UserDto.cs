using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.ApplicationUsers
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
