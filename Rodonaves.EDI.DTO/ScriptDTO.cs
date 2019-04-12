using Rodonaves.EDI.Enums;

namespace Rodonaves.EDI.DTO
{
    public class ScriptDTO : BaseDTO
    {
        public int ProviderId { get; set; }

        public ProviderDTO Provider { get; set; }

        public string Description { get; set; }
        public string Script { get; set; }
        public string BandType { get; set; }
        public ProcessTypeEnum ProcessType { get; set; }

        private string processTypeName;
        public string ProcessTypeName
        {
            get
            {
                if (string.IsNullOrEmpty(processTypeName))
                    processTypeName = ProcessType.ToString();

                return processTypeName;
            }
            set { processTypeName = value; }
        }
    }
}
