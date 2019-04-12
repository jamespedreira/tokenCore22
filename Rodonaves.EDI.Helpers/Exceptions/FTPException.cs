using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Helpers.Exceptions
{
    public class FTPException : Exception
    {
        public FTPException() { }

        public FTPException(string message) : base(message) { }

        public FTPException(string message, Exception inner) : base(message, inner) { }
    }
}
