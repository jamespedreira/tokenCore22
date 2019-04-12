

using Rodonaves.EDI.Enums;

namespace Rodonaves.EDI.DTO
{
    public class OperationDetailedDTO
    {
        public int TotalItems { get; set; }
        public DeliveryProgressEnum ProgressStatus { get; set; }
    }
}