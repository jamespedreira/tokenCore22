using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface ILogDal : IBaseCrudDal<LogInfo>
    {
        Task<List<LogInfo>> GetByGenerateReturnIdAsync(long generateReturnId);
    }
}
