using System.Collections.Generic;
using System.Threading.Tasks;
using Rodonaves.EDI.DTO;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IGenerateReturnDetailedStatus : IBaseCrud<GenerateReturnDetailedStatusDTO>
    {
        Task<List<GenerateReturnDetailedStatusDTO>> GetAllAsync();
        Task<List<GenerateReturnDetailedStatusDTO>> GetResumeByProcessTypeAsync();

        Task<List<GenerateReturnDetailedStatusDTO>> GetByParamsAsync(GenerateReturnDetailedStatusDTO dto, int page, int amountByPage);
        Task<List<LogDTO>> GetLogByGenerateReturnIdAsync(long generatereturnId);

        Task<List<OperationDetailedDTO>> GetTotalOperationByStatusAsync();

        Task<List<QueueDTO>> GetQueueStatus();
    }
}