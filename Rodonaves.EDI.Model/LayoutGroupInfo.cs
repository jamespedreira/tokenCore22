using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class LayoutGroupInfo : BaseInfo
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string DevelopmentCompany { get; set; }
        public int? AmountPages { get; set; }
    }
}
