using Rodonaves.EDI.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.DTO
{
    public class GenerateReturnDTO : BaseDTO
    {
        public LayoutHeaderDTO LayoutHeader { get; set; } = new LayoutHeaderDTO();

        public LayoutFileNameDTO LayoutFileName { get; set; } = new LayoutFileNameDTO();

        public CustomerDTO Customer { get; set; } = new CustomerDTO();

        public AgreementProcessDTO AgreementProcess { get; set; } = new AgreementProcessDTO();

        public ProcessTypeEnum ProcessType { get; set; }

        public DateTime StartingDate { get; set; }

        public DateTime EndingDate { get; set; }

        public int BillOfLadingId { get; set; }

        public ExecutionTypeEnum ExecutionType { get; set; }

        public ProgressStatusEnum ProgressStatus { get; set; }

        public int BillOfLadingQuantity { get; set; }

        public List<CommunicationChannelDTO> CommunicationChannels { get; set; } = new List<CommunicationChannelDTO>();
    }
}
