using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.Interfaces;

namespace Rodonaves.EDI.Infra.Factories
{
    public class ExplainPlanFactory : IExplainPlanFactory
    {
        public IExplainPlan CreateNew(ProviderTypeEnum providerType, string connectionString, string query)
        {
            switch (providerType)
            {
                case ProviderTypeEnum.PostgreSQL:
                    return new Rodonaves.EDI.Helpers.PostgreSQL.ExplainPlanHelper(connectionString, query, Rodonaves.EDI.Helpers.PostgreSQL.Enums.ExplainPlanFormattingType.YAML);
                case ProviderTypeEnum.Oracle:
                default:
                    return new Rodonaves.EDI.Helpers.Oracle.ExplainPlanHelper(connectionString, query, Rodonaves.EDI.Helpers.Oracle.Enums.ExplainPlanFormattingType.All);
            }
        }
    }
}
