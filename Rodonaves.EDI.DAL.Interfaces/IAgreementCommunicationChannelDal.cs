using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IAgreementCommunicationChannelDal : IBaseCrudDal<AgreementCommunicationChannelInfo>
    {
        Task<List<AgreementCommunicationChannelInfo>> GetAllAsync();

        Task<List<AgreementCommunicationChannelInfo>> GetByParamsAsync(AgreementCommunicationChannelInfo info, int page, int amountByPage);

        Task<List<AgreementCommunicationChannelInfo>> GetByAgreementProcessAsync(int agreementProcessId);

        AgreementCommunicationChannelInfo NewClassInfo(object reader);
                
    }
}
