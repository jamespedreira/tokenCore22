using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using System.Collections.Generic;

namespace Rodonaves.EDI.Infra.Factories
{
    public class ColumnHeaderFactory : IColumnHeaderFactory
    {

        public IColumnHeader CreateNew(ProviderTypeEnum providerType, string connectionString, string query, List<QueryParameter> parameters)
        {
            switch (providerType)
            {
                case ProviderTypeEnum.PostgreSQL:
                    return new Rodonaves.EDI.Helpers.PostgreSQL.ColumnHeaderHelper(connectionString, query, parameters);
                case ProviderTypeEnum.Oracle:
                default:
                    return new Rodonaves.EDI.Helpers.Oracle.ColumnHeaderHelper(connectionString, query, parameters);
            }
        }
    }
}
