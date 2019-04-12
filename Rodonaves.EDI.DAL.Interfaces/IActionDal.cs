using System.Collections.Generic;
using System.Threading.Tasks;
using Rodonaves.EDI.Model;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IActionDal : IBaseCrudDal<ActionInfo>
    {
        Task<List<ActionInfo>> GetByTaskIdAsync (int taskId);
    }
}