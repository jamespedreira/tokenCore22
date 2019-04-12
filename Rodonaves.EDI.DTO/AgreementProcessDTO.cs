using Rodonaves.EDI.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.DTO
{
    public class AgreementProcessDTO : BaseDTO
    {
        public int AgreementId { get; set; }
        public ProcessTypeEnum ProcessType { get; set; }
        public int LayoutHeaderId { get; set; }
        public PeriodicityTypeEnum PeriodicityType { get; set; }
        public int LayoutFileNameId { get; set; }
        public List<AgreementCommunicationChannelDTO> CommunicationChannels { get; set; }

        public DateTime LastRun { get; set; }
        public DateTime NextRun { get; set; }

        public AgreementProcessDTO()
        {
            CommunicationChannels = new List<AgreementCommunicationChannelDTO>();
        }
    }
}
