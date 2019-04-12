using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.QueueMessage.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IGenerateReturn : IBaseCrud<GenerateReturnDTO>
    {
        IRTEQueue CreateQueue(string queueName, out int maxPriority);

        Task<int> InsertAsync(GenerateReturnDTO dto, string queueName);

        Task<bool> UpdateProgressStatusAsync(int generateReturnId, ProgressStatusEnum progressStatus);

        Task<List<GenerateReturnDTO>> GetAllAsync();
        
        Task<List<GenerateReturnDTO>> GetByParamsAsync(GenerateReturnDTO dto, int page, int amountByPage);

        Task<List<GenerateReturnDTO>> GetUnprocessedGenerateReturnAsync();

        Task<List<GenerateReturnDTO>> GetUnprocessedGenerateReturnByCustomerAsync(int customerId);

        Task<GenerateReturnDTO> GetByIdAsync(int id);

        Task InsertGenerateReturnValuesAsync(GenerateReturnDTO dto, QueryResult<object> resultValues);

        Task UpdateBillOfLadingQuantaty(int id);
    }
}
