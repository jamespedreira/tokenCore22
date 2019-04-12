using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Model;
using RTEFramework.BLL.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class LayoutGroup : BaseCrudBll<ILayoutGroupDal, LayoutGroupDTO, LayoutGroupInfo>, ILayoutGroup
    {
        #region Ctor

        private ILayoutGroupDal _layoutGroupDal;

        public LayoutGroup(ILayoutGroupDal layoutGroupDal, IProviderDal providerDal, IMapper mapper) : base(layoutGroupDal, mapper)
        {
            _layoutGroupDal = layoutGroupDal;
        }

        #endregion

        public async Task<List<LayoutGroupDTO>> GetAllAsync()
        {
            var list = await _dal.GetAll();

            return _mapper.Map<List<LayoutGroupInfo>, List<LayoutGroupDTO>>(list);
        }

        public new async Task<int> InsertAsync(LayoutGroupDTO dto)
        {
            bool success = await base.InsertAsync(dto);

            if (!success)
                throw new BusinessException("Não foi possível inserir o grupo de layout.");

            return dto.Id;
        }

        public override async Task<bool> UpdateAsync(LayoutGroupDTO dto)
        {
            bool success = await base.UpdateAsync(dto);

            if (!success)
                throw new BusinessException("Não foi possível editar o grupo de layout.");

            return success;
        }

        public override async Task<bool> DeleteAsync(int id)
        {

            bool success = false;
            success = await base.DeleteAsync(id);

            if (!success)
                throw new BusinessException("Não foi possível deletar o grupo de layout.");

            return success;
        }

        public async Task<List<LayoutGroupDTO>> GetByParamsAsync(int id, string description, string developmentCompany, int page, int amountByPage)
        {
            var list = await _dal.GetByParamsAsync(id, description, developmentCompany, page, amountByPage);

            return _mapper.Map<List<LayoutGroupInfo>, List<LayoutGroupDTO>>(list);
        }

    }
}
