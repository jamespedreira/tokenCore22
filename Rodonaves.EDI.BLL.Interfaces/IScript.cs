using Rodonaves.EDI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IScript : IBaseCrud<ScriptDTO>
    {
        Task<List<ScriptDTO>> GetAllAsync();

        Task<ScriptDTO> GetByIdAsync(int id);

        Task<List<ScriptDTO>> GetByParamsAsync(int? id, int? providerId, string description, string script, string bandType, string processType, int page, int amountByPage);
        Task<ExplainPlanDTO> GetExplainPlan(int providerId, string query);
        Task<List<ScriptDTO>> GetByProcessTypeAsync(string processType);
        Task<List<ScriptDTO>> GetByBandTypeAsync(string processType, string bandType);
        Task<string[]> RetrieveColumns(int query);
    }
}
