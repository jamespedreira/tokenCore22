using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IAgreementDal : IBaseCrudDal<AgreementInfo>
    {
        Task<List<AgreementInfo>> GetAll(long companyId);
        Task<List<AgreementInfo>> GetByParamsAsync(AgreementInfo info, bool? active, int page, int amountByPage);

        Task<List<AgreementInfo>> GetAgreementToExport(DateTime referenceDate);

        AgreementInfo NewClassInfo(object reader);
    }
}
