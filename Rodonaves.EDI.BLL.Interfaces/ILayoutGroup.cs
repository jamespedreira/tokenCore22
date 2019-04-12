using Rodonaves.EDI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface ILayoutGroup : IBaseCrud<LayoutGroupDTO>
    {
        Task<List<LayoutGroupDTO>> GetAllAsync();
        Task<List<LayoutGroupDTO>> GetByParamsAsync(int id, string description, string developmentCompany, int page, int amountByPage);
    }
}
