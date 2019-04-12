using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IGenerateReturnValueDal : IBaseCrudDal<GenerateReturnValueInfo>
    {
        Task<List<GenerateReturnValueInfo>> GetAllAsync();

        Task<List<GenerateReturnValueInfo>> GetByGenerateReturnIdAsync(int generateReturnId);

        Task<List<GenerateReturnValueInfo>> GetByGenerateReturnIdAsync(int generateReturnId, int page, int amountByPage);
    }
}
