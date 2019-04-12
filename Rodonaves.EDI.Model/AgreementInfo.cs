using Rodonaves.Engine.BaseObjects;
using System.Collections.Generic;

namespace Rodonaves.EDI.Model
{
    public class AgreementInfo : BaseInfo
    {
        public int Id { get; set; }
        public CustomerInfo Customer { get; set; }
        public bool Active { get; set; }
        public bool Raiz { get; set; }
        public long CompanyId { get; set; }
        public int? AmountPages { get; set; }
        public List<AgreementProcessInfo> Processes { get; set; }
        public List<AgreementOccurrenceInfo> Occurrences { get; set; }

        public AgreementInfo()
        {
            Active = true;
            Customer = new CustomerInfo();
            Processes = new List<AgreementProcessInfo>();
            Occurrences = new List<AgreementOccurrenceInfo>();
        }
    }
}
