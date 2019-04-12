using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.DTO
{
    public class BaseDTO
    {
        public int Id { get; set; }
        public int? Revision { get; set; }
        public int? AmountPages { get; set; }
    }
}
