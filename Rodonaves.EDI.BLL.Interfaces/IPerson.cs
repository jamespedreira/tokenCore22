using Rodonaves.EDI.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IPerson : IBaseCrud<PersonDTO>
    {
        Task<List<PersonDTO>> GetAllAsync(long companyId);
        Task<List<PersonDTO>> GetPersonExternal(int? page, int? amountByPage, string description, string taxIdRegistration);
    }
}
