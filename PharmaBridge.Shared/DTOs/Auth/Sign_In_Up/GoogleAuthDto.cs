using PharmaBridge.Shared.EnumHelper.UserEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Auth.Sign_In_Up
{
    public class GoogleAuthDto
    {
        public string IdToken { get; set; } = null!;
        public UserRole? Role { get; set; }
    }
}
