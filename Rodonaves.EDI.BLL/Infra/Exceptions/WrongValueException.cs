using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Rodonaves.EDI.BLL.Infra.Exceptions
{
    public class WrongValueException : Exception
    {
        public WrongValueException()
        {
        }

        public WrongValueException(string message) : base(message)
        {
        }

        public WrongValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
