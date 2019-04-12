using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.Interfaces;

namespace Rodonaves.EDI.BLL.Interfaces.Factories
{
    public interface IExplainPlanFactory
    {
        IExplainPlan CreateNew(ProviderTypeEnum providerType, string connectionString, string query);
    }
}
