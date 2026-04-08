using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Dto_s.Token
{
    public class TokenResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpireOn { get; set; }
    }
}
