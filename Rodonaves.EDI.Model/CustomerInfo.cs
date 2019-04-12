using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Model
{
    public class CustomerInfo : BaseInfo
    {
        public int Id { get; set; }        
        public long CompanyId { get; set; }
        public int IdPerson { get; set; }

        public PersonInfo Person { get; set; } = new PersonInfo();
    }
}
