using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Dtos.UserProfiles
{
    public class UserDto
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
