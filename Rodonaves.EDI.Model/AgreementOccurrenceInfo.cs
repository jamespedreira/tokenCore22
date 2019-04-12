using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Model
{
    public class AgreementOccurrenceInfo : BaseInfo
    {
        public int Id { get; set; }
        public int AgreementId { get; set; }
        public int NatureOccurrenceId { get; set; }

    }
}
