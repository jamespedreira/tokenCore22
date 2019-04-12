using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Rodonaves.EDI.DTO
{
    public class PersonDTO : BaseDTO
    {
        public string Description { get; set; }
        public string TaxIdRegistration { get; set; }

        public virtual string DescriptionTaxIdRegistration => Description + " - " + TaxIdRegistration.FormatToTaxId();

    }
}

