using Rodonaves.EDI.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface ICustomer : IBaseCrud<CustomerDTO>
    {
        Task<List<CustomerDTO>> GetAllAsync(long companyId);

        Task<CustomerDTO> GetByIdAsync(int id);

        Task<List<CustomerDTO>> GetByParams(CustomerDTO dto, int page, int amountByPage);

        Task<List<CustomerDTO>> GetByProcessType(long companyId, int processType, int page, int amountByPage);

        int ExistCustomerId(string taxIdRegistration, long companyId);
    }
}
