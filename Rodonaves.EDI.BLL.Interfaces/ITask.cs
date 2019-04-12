using Rodonaves.EDI.DTO;
using Rodonaves.TaskExecutor.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface ITask : IBaseCrud<TaskDTO>
    {
        Task<List<TaskDTO>> GetAllAsync();
        Task<List<TaskDTO>> GetByParamsAsync(int id, string name, string descripton, int page, int amountByPage);
        Task<List<Rodonaves.TaskExecutor.Infra.TaskInfo>> LoadTasksAsync();
    }
}
