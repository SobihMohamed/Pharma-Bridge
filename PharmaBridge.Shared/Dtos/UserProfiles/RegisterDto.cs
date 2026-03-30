using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Dtos.UserProfiles
{
    public class RegisterDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }


    }
}
