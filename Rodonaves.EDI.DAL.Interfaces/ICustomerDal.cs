using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface ICustomerDal : IBaseCrudDal<CustomerInfo>
    {
        int ExistCustomerId(string taxIdRegistration, long companyId);

        Task<List<CustomerInfo>> GetAll(long companyId);
        Task<List<CustomerInfo>> GetByParams(CustomerInfo info, int page, int amountByPage);

        Task<List<CustomerInfo>> GetByProcessType(long companyId, int processType, int page, int amountByPage);

        CustomerInfo NewClassInfo(object reader);
    }
}
