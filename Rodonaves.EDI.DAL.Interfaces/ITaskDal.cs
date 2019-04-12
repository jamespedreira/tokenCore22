using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface ITaskDal : IBaseCrudDal<TaskInfo>
    {
        Task<List<TaskInfo>> GetAll();
        Task<List<TaskInfo>> GetByParamsAsync(int id, string name, string descripton, int page, int amountByPage);

    }
}
