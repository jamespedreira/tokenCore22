using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class OperationDetailedInfo : BaseInfo
    {
        public int TotalItems { get; set; }
        public DeliveryProgressEnum ProgressStatus { get; set; }
    }
}