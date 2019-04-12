using System.Collections.Generic;
using System.Threading.Tasks;
using Rodonaves.EDI.Model;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IGenerateReturnDetailedStatusDal : IBaseCrudDal<GenerateReturnDetailedStatusInfo>
    {
        Task<List<GenerateReturnDetailedStatusInfo>> GetAllAsync();
        Task<List<GenerateReturnDetailedStatusInfo>>  GetResumeByProcessTypeAsync();
        Task<List<GenerateReturnDetailedStatusInfo>> GetByParamsAsync(GenerateReturnDetailedStatusInfo info, int page, int amountByPage);
    }
}