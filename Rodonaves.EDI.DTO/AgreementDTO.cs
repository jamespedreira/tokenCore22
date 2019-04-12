using System.Collections.Generic;

namespace Rodonaves.EDI.DTO
{
    public class AgreementDTO : BaseDTO
    {
        public CustomerDTO Customer { get; set; }        
        public long CompanyId { get; set; }
        public bool Active { get; set; }
        public bool Raiz { get; set; }
        public List<AgreementProcessDTO> Processes { get; set; }
        public List<AgreementOccurrenceDTO> Occurrences { get; set; }

        public AgreementDTO()
        {
            Processes = new List<AgreementProcessDTO>();
            Occurrences = new List<AgreementOccurrenceDTO>();
        }
    }
}
