using AutoMapper;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using RTEFramework.BLL.Infra;
using System.Data;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public abstract class BaseCrudBll<TIDal, TDto, TInfo> where TInfo: BaseInfo
                                             where TIDal: IBaseCrudDal<TInfo>
                                             where TDto: BaseDTO
    {
        protected readonly TIDal _dal;
        protected readonly IMapper _mapper;

        protected BaseCrudBll(TIDal dal, IMapper mapper)
        {
            this._dal = dal;
            this._mapper = mapper;
        }

        public virtual async Task<bool> InsertAsync(TDto dto)
        {
            dto.Id = _dal.GetNextId();
            bool success = await _dal.InsertAsync(_mapper.Map<TInfo>(dto));
            return success;
        }
        public virtual async Task<bool> InsertAsync(TDto dto, IDbTransaction transaction)
        {
            dto.Id = _dal.GetNextId();
            bool success = await _dal.InsertAsync(_mapper.Map<TInfo>(dto), transaction);
            return success;
        }

        public virtual async Task<bool> UpdateAsync(TDto dto)
        {
            bool success = await _dal.UpdateAsync(_mapper.Map<TInfo>(dto));
            return success;
        }

        public virtual async Task<bool> UpdateAsync(TDto dto, IDbTransaction transaction)
        {
            bool success = await _dal.UpdateAsync(_mapper.Map<TInfo>(dto), transaction);
            return success;
        }


        public virtual async Task<bool> DeleteAsync(int id)
        {

            bool success = false;
            var info = await _dal.GetByIdAsync(id);

            if (info == null)
                throw new BusinessException("Registro não encontrado");
            else
                success = await _dal.DeleteAsync(info);

            return success;
        }

        public virtual async Task<bool> DeleteAsync(int id, IDbTransaction transaction)
        {

            bool success = false;
            var info = await _dal.GetByIdAsync(id);

            if (info == null)
                throw new BusinessException("Registro não encontrado");
            else
                success = await _dal.DeleteAsync(info, transaction);

            return success;
        }



    }
}
