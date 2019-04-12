using System.Collections.Generic;
using System.Threading.Tasks;
using Rodonaves.EDI.DTO;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IAction  : IBaseCrud<ActionDTO>
    {
         Task<List<ActionDTO>> GetByTaskIdAsync (int taskId);
    }
}