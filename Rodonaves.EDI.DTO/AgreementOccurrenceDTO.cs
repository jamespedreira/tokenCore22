using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.DTO
{
    public class AgreementOccurrenceDTO : BaseDTO
    {
        public int AgreementId { get; set; }
        public int NatureOccurrenceId { get; set; }
    }
}
