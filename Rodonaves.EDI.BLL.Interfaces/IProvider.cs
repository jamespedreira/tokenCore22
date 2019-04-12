using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IProvider : IBaseCrud<ProviderDTO>
    {
        Task<List<ProviderDTO>> GetAllAsync();
        Task<ProviderDTO> GetByIdAsync(int id);
        Task<List<ProviderDTO>> GetByParamsAsync(int id, int? providerType,string name, string descripton, string connectionString, int page, int amountByPage);
        Task<TestConnectionDTO> TestConnection(string connectionString, ProviderTypeEnum provider);
    }
}
