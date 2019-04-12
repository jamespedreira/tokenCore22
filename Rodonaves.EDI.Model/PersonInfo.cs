using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Model
{
    public class PersonInfo : BaseInfo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string TaxIdRegistration { get; set; }

        public int? AmountPages { get; set; }
        //public List<CustomerInfo> Custormes { get; set; }

        //public PersonInfo()
        //{
        //    Custormes = new List<CustomerInfo>();
        //}

    }
}
