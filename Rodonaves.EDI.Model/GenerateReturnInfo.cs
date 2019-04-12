using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Model
{
    public class GenerateReturnInfo : BaseInfo
    {
        public int Id { get; set; }

        public LayoutHeaderInfo LayoutHeader { get; set; } = new LayoutHeaderInfo();

        public LayoutFileNameInfo LayoutFileName { get; set; } = new LayoutFileNameInfo();

        public CustomerInfo Customer { get; set; } = new CustomerInfo();

        public AgreementProcessInfo AgreementProcess { get; set; } = new AgreementProcessInfo();

        public ProcessTypeEnum ProcessType { get; set; }

        public DateTime StartingDate { get; set; }

        public DateTime EndingDate { get; set; }

        public int AmountPages { get; set; }

        public int BillOfLadingId { get; set; }

        public ExecutionTypeEnum ExecutionType { get; set; }

        public ProgressStatusEnum ProgressStatus { get; set; }

        public int BillOfLadingQuantity { get; set; }

        public List<CommunicationChannelInfo> CommunicationChannels { get; set; } = new List<CommunicationChannelInfo>();

        public List<GenerateReturnValueInfo> GenerateReturnValues { get; set; } = new List<GenerateReturnValueInfo>();
    }
}
