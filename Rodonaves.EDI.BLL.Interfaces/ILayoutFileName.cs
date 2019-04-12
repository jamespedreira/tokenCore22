using Rodonaves.EDI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface ILayoutFileName : IBaseCrud<LayoutFileNameDTO>
    {
        Task<List<LayoutFileNameDTO>> GetAllAsync();

        Task<LayoutFileNameDTO> GetByIdAsync(int id);

        Task<List<LayoutFileNameDTO>> GetByParams(LayoutFileNameDTO dto, int page, int amountByPage);

        Task<List<LayoutFileNameDTO>> GetByProcessType(long companyId, int processType, int customerId,int page, int amountByPage);
    }
}
