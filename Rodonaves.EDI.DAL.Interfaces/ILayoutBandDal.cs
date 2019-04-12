using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface ILayoutBandDal : IBaseCrudDal<LayoutBandInfo>
    {
        Task<List<LayoutBandInfo>> GetByLayoutHeaderAsync(long layoutHeaderId);
    }
}
