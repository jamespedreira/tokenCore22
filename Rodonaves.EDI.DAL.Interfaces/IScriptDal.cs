using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IScriptDal : IBaseCrudDal<ScriptInfo>
    {
        Task<List<ScriptInfo>> GetAll();
        Task<List<ScriptInfo>> GetByParamsAsync(int? id, int? providerId, string description, string script, string bandType, string processType, int page, int amountByPage);
        Task<List<ScriptInfo>> GetByProcessTypeAsync(string processType);
        Task<List<ScriptInfo>> GetByBandTypeAsync(string processType, string bandType);

        ScriptInfo NewClassInfo(object reader);
    }
}
