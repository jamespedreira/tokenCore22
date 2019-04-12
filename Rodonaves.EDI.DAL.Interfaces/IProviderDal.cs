using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IProviderDal : IBaseCrudDal<ProviderInfo>
    {
        Task<List<ProviderInfo>> GetAll();

        Task<List<ProviderInfo>> GetByParamsAsync(int id, int? providerType, string name, string descripton, string connectionString, int page, int amountByPage);

    }
}
