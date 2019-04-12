using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IDetailedDayDal : IBaseCrudDal<DetailedDayInfo>
    {
        Task<List<int>> GetDaysByConfigurationIdAsync(int configurationId);
        Task<bool> DeleteByConfigurationId(int configurationId);
    }
}
