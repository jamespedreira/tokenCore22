using Rodonaves.EDI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IAgreement : IBaseCrud<AgreementDTO>
    {
        Task<AgreementDTO> GetByIdAsync(int id);
        Task<List<AgreementDTO>> GetAllAsync(long companyId);
        Task<List<AgreementDTO>> GetByParamsAsync(AgreementDTO dto, bool? active, int page, int amountByPage);
        Task<List<AgreementDTO>> GetAgreementToExport();

        Task<bool> SetLastRunToProcess(int processId, DateTime lastRun);
    }
}
