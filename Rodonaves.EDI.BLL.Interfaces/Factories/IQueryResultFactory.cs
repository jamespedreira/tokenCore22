using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using System.Collections.Generic;

namespace Rodonaves.EDI.BLL.Interfaces.Factories
{
    public interface IQueryResultFactory
    {
        IQueryResultHelper<object> CreateNew(ProviderTypeEnum providerType, string connectionString, string query, List<QueryParameter> parameters);
    }
}
