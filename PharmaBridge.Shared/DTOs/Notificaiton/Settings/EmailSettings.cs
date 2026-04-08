using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Dto_s.Notificaiton.Settings
{
    public class EmailSettings
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Host { get; set; } = null!;
        public int Port { get; set; }
    }
}
