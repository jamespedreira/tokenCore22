using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IAgreementProcessDal : IBaseCrudDal<AgreementProcessInfo>
    {
        Task<List<AgreementProcessInfo>> GetByAgreementAsync(int agreementId);

        Task<List<AgreementProcessInfo>> GetAgreementProcessToExport(int agreementId, DateTime referenceDate);
    }
}
