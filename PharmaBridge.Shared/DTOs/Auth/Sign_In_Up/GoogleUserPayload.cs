using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Auth.Sign_In_Up
{
    public class GoogleUserPayload
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public bool Email_Verified { get; set; }
    }
}
