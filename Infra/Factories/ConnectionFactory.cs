using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.Enums;
using IConnection = Rodonaves.EDI.Helpers.Interfaces.IConnection;

namespace Rodonaves.EDI.Infra.Factories
{
    public class ConnectionFactory : IConnectionFactory
    {
        public IConnection CreateNew(ProviderTypeEnum providerType, string connectionString)
        {
            switch (providerType)
            {
                case ProviderTypeEnum.PostgreSQL:
                    return new Helpers.PostgreSQL.ConnectionHelper(connectionString);
                case ProviderTypeEnum.Oracle:
                default:
                    return new Helpers.Oracle.ConnectionHelper(connectionString);
            }
        }
    }
}
