using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Exceptions.NotFoundHandeler.Request
{
    public class RequestNotFoundException(string msg) : NotFoundCutomeException(msg)
    {
    }
}
