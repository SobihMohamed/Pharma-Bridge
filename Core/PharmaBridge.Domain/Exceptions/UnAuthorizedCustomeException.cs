using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Exceptions
{
    public class UnAuthorizedCustomeException(string message = "Invalid Operation") : Exception(message)
    {
    }
}
