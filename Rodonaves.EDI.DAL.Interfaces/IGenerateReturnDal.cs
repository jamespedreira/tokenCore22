using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IGenerateReturnDal : IBaseCrudDal<GenerateReturnInfo>
    {
        Task<List<GenerateReturnInfo>> GetAllAsync();

        Task<List<GenerateReturnInfo>> GetByParamsAsync(GenerateReturnInfo info, int page, int amountByPage);

        Task<List<GenerateReturnInfo>> GetUnprocessedGenerateReturnAsync();

        Task<List<GenerateReturnInfo>> GetUnprocessedGenerateReturnByCustomerAsync(int customerId);

        Task UpdateBillOfLadingQuantaty(int id);
    }
}
