using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;

namespace Rodonaves.EDI.Model
{
    public class AgreementProcessInfo : BaseInfo
    {
        public int Id { get; set; }
        public int AgreementId { get; set; }
        public ProcessTypeEnum ProcessType { get; set; }
        public int LayoutHeaderId { get; set; }
        public PeriodicityTypeEnum PeriodicityType { get; set; }
        public int LayoutFileNameId { get; set; }

        public DateTime LastRun { get; set; }
        public DateTime NextRun { get; set; }

        public List<AgreementCommunicationChannelInfo> CommunicationChannels { get; set; }

        public AgreementProcessInfo()
        {
            CommunicationChannels = new List<AgreementCommunicationChannelInfo>();
        }
    }

}
