using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.ApplicationUsers
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }


    }
}
