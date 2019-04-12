using Rodonaves.EDI.Enums;

namespace Rodonaves.EDI.DTO
{
    public class ProviderDTO : BaseDTO
    {
        public string Name { get; set; }

        public ProviderTypeEnum ProviderType { get; set; }

        private string providerTypeName;

        public string ProviderTypeName
        {
            get {
                if (string.IsNullOrEmpty(providerTypeName))
                    providerTypeName = ProviderType.ToString();

                return providerTypeName;
            }
            set { providerTypeName = value; }
        }

        public string Description { get; set; }

        public string ConnectionString { get; set; }
    }
}
