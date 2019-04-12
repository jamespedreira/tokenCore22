using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IDetailedMonthDal : IBaseCrudDal<DetailedMonthInfo>
    {
        Task<List<int?>> GetMonthsByConfigurationIdAsync(int configurationId);
        Task<bool> DeleteByConfigurationId(int configurationId);
    }
}
