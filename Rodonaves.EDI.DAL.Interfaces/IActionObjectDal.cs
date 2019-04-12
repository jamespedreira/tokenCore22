using System.Collections.Generic;
using System.Threading.Tasks;
using Rodonaves.EDI.Model;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IActionObjectDal : IBaseCrudDal<ActionObjectInfo>
    {
        Task<List<ActionObjectInfo>> GetByActionIdAsync (int actionId);
        Task<bool> DeleteByActionAsync (int actionId);

    }
}