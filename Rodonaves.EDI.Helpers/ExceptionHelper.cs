using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Helpers
{
    public static class ExceptionHelper
    {
        public static Exception GetDeepestException(Exception ex)
        {
            return ex.InnerException != null ? GetDeepestException(ex.InnerException) : ex;
        }
    }
}
