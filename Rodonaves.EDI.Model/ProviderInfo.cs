using Rodonaves.Engine.BaseObjects;
using Rodonaves.EDI.Enums;

namespace Rodonaves.EDI.Model
{
    public class ProviderInfo : BaseInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProviderTypeEnum ProviderType { get; set; }
        public string Description { get; set; }
        public string ConnectionString { get; set; }
        public int? AmountPages { get; set; }
    }
}