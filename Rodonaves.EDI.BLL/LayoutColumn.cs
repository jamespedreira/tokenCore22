using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class LayoutColumn : BaseCrudBll<ILayoutColumnDal, LayoutColumnDTO, LayoutColumnInfo>, ILayoutColumn
    {
        public LayoutColumn(ILayoutColumnDal dal, IMapper mapper) : base(dal, mapper) { }

        async Task<int> IBaseCrud<LayoutColumnDTO>.InsertAsync(LayoutColumnDTO dto)
        {
            await base.InsertAsync(dto);

            return dto.Id;
        }
    }
}
