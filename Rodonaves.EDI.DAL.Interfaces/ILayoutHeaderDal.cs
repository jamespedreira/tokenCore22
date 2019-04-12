using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface ILayoutHeaderDal : IBaseCrudDal<LayoutHeaderInfo>
    {
        Task<List<LayoutHeaderInfo>> GetAllAsync(long companyId);

        Task<List<LayoutHeaderInfo>> GetByParamsAsync(int id, long? layoutGroupId, string description, bool? active, long? scriptId, long companyId, string processType, int page, int amountByPage);

        Task<List<LayoutHeaderInfo>> GetByProcessType(long companyId, int processType, int customerId, int page, int amountByPage);
    }
}
