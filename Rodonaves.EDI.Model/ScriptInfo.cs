using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class ScriptInfo : BaseInfo
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }

        public ProviderInfo Provider { get; set; }

        public string Description { get; set; }
        public string Script { get; set; }
        public string BandType { get; set; }
        public ProcessTypeEnum ProcessType { get; set; }
        public int? AmountPages { get; set; }
    }
}
