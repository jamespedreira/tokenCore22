using Rodonaves.EDI.Enums;

namespace Rodonaves.EDI.BLL.Interfaces.Factories
{
    public interface IConnectionFactory
    {
        Helpers.Interfaces.IConnection CreateNew(ProviderTypeEnum providerType, string connectionString);
    }
}
