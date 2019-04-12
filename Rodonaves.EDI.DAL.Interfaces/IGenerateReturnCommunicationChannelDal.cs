using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IGenerateReturnCommunicationChannelDal : IBaseCrudDal<CommunicationChannelInfo>
    {
        Task<List<CommunicationChannelInfo>> GetAllAsync();

        Task<List<CommunicationChannelInfo>> GetByFileReturnAsync(int FileReturnId);

        Task<List<CommunicationChannelInfo>> GetByParamsAsync(CommunicationChannelInfo info, int page, int amountByPage);

        CommunicationChannelInfo NewClassInfo(object reader);
    }
}
