
using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using System.Collections.Generic;

namespace Rodonaves.EDI.BLL.Infra.Factories
{
    public class QueryResultFactory : IQueryResultFactory
    {
        public QueryResultFactory()
        {
        }

        public IQueryResultHelper<object> CreateNew(ProviderTypeEnum providerType, string connectionString, string query, List<QueryParameter> parameters)
        {
            switch (providerType)
            {
                case ProviderTypeEnum.PostgreSQL:
                    return new Helpers.PostgreSQL.QueryResultHelper<object>(connectionString, query, parameters);
                case ProviderTypeEnum.Oracle:
                default:
                    return new Helpers.Oracle.QueryResultHelper<object>(connectionString, query, parameters);
            }
        }
    }
}
