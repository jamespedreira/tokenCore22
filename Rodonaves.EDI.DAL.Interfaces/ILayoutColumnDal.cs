using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rodonaves.EDI.Model;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface ILayoutColumnDal : IBaseCrudDal<LayoutColumnInfo>
    {
        Task<List<LayoutColumnInfo>> GetByLayoutBandAsync (long layoutBandId);
        Task<List<LayoutColumnInfo>> GetByLayoutFileNameAsync (long layoutFileNameId);

    }
}