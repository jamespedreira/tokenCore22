using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class LayoutFileName : BaseCrudBll<ILayoutFileNameDal, LayoutFileNameDTO, LayoutFileNameInfo>, ILayoutFileName
    {
        private readonly ILayoutColumnDal _layoutColumnDal;
        private readonly ILayoutDictionaryDal _layoutDictionaryDal;

        public LayoutFileName(
            ILayoutFileNameDal dal, 
            IMapper mapper, 
            ILayoutColumnDal layoutColumnDal,
            ILayoutDictionaryDal layoutDictionaryDal) : base(dal, mapper){
            _layoutColumnDal = layoutColumnDal;
            _layoutDictionaryDal = layoutDictionaryDal;
        }

        public async Task<List<LayoutFileNameDTO>> GetAllAsync()
        {
            var list = await _dal.GetAllAsync();
            return _mapper.Map<List<LayoutFileNameInfo>, List<LayoutFileNameDTO>>(list);
        }

        public async Task<LayoutFileNameDTO> GetByIdAsync(int id)
        {
            var info = await _dal.GetByIdAsync(id);

            info = await GetLayoutFileNameDependecies(info);

            return _mapper.Map<LayoutFileNameInfo, LayoutFileNameDTO>(info);
        }

        public async Task<List<LayoutFileNameDTO>> GetByParams(LayoutFileNameDTO dto, int page, int amountByPage)
        {
            var info = _mapper.Map<LayoutFileNameDTO, LayoutFileNameInfo>(dto);
            var list = await _dal.GetByLParamsAsync(info, page, amountByPage);
            return _mapper.Map< List<LayoutFileNameInfo>,List<LayoutFileNameDTO>>(list);
        }

        public async Task<List<LayoutFileNameDTO>> GetByProcessType(long companyId, int processType, int customerId, int page, int amountByPage)
        {
            var list = await _dal.GetByProcessType(companyId, processType, customerId, page, amountByPage);

            return _mapper.Map<List<LayoutFileNameInfo>, List<LayoutFileNameDTO>>(list);
        }

        async Task<int> IBaseCrud<LayoutFileNameDTO>.InsertAsync(LayoutFileNameDTO dto)
        {
            await base.InsertAsync(dto);

            return dto.Id;
        }

        async Task<LayoutFileNameInfo> GetLayoutFileNameDependecies(LayoutFileNameInfo info)
        {
            info.LayoutColumns = await _layoutColumnDal.GetByLayoutFileNameAsync(info.Id);

            foreach (var col in info.LayoutColumns)
                col.LayoutDictionaries = await _layoutDictionaryDal.GetByLayoutColumnIdAsync(col.Id);

            return info;
        }
    }
}
