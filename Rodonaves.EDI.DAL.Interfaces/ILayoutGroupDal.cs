using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface ILayoutGroupDal : IBaseCrudDal<LayoutGroupInfo>
    {
        Task<List<LayoutGroupInfo>> GetAll();
        Task<List<LayoutGroupInfo>> GetByParamsAsync(int id, string description, string developmentCompany, int page, int amountByPage);
    }
}
