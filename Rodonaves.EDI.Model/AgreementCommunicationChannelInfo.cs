using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Model
{
    public class AgreementCommunicationChannelInfo : CommunicationChannelInfo
    {
        public int AgreementProcessId { get; set; }
    }
}
