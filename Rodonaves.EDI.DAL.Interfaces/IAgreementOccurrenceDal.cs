using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IAgreementOccurrenceDal : IBaseCrudDal<AgreementOccurrenceInfo>
    {
        Task<List<AgreementOccurrenceInfo>> GetByAgreementAsync(int agreementId);
    }
}
