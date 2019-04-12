using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Model
{
    public class GenerateReturnValueInfo : BaseInfo
    {
        public int Id { get; set; }

        public int GenerateReturnId { get; set; }

        public string Value { get; set; }
    }
}
