using System.Collections.Generic;
using System.Threading.Tasks;
using Rodonaves.EDI.Model;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface ILayoutFileNameDal : IBaseCrudDal<LayoutFileNameInfo>
    {
        Task<List<LayoutFileNameInfo>> GetAllAsync();

        Task<List<LayoutFileNameInfo>> GetByLayoutHeaderIdAsync(long layoutHeaderId);

        Task<List<LayoutFileNameInfo>> GetByLParamsAsync(LayoutFileNameInfo info, int page, int amountByPage);

        Task<List<LayoutFileNameInfo>> GetByProcessType(long companyId, int processType, int customerId, int page, int amountByPage);
    }
}