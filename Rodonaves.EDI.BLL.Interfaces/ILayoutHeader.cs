using Rodonaves.EDI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface ILayoutHeader : IBaseCrud<LayoutHeaderDTO>
    {
        Task<List<LayoutHeaderDTO>> GetAllAsync(long companyId);

        Task<LayoutHeaderDTO> GetByIdAsync(int id);

        Task<List<LayoutHeaderDTO>> GetByParamsAsync(int id, long? layoutGroupId, string description, bool? active, long? scriptId, long companyId, string processType, int page, int amountByPage);

        Task<List<LayoutHeaderDTO>> GetByProcessType(long companyId, int processType, int customerId, int page, int amountByPage);
    }
}
