using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface ITriggerDal : IBaseCrudDal<TriggerInfo>
    {
        Task<List<TriggerInfo>> GetAll();
        Task<List<TriggerInfo>> GetByParamsAsync(int id, int taskId, int? frequence, int page, int amountByPage);
        Task<List<TriggerInfo>> GetByTaskIdAsync(int taskId);
    }
}
