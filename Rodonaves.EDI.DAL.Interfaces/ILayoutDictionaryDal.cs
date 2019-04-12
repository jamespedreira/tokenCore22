using System.Collections.Generic;
using System.Threading.Tasks;
using Rodonaves.EDI.Model;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface ILayoutDictionaryDal : IBaseCrudDal<LayoutDictionaryInfo>
    {
        Task<List<LayoutDictionaryInfo>> GetByLayoutColumnIdAsync (int layoutColumnId);
    }
}