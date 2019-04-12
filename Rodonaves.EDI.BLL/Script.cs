using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Model;
using RTEFramework.BLL.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class Script : BaseCrudBll<IScriptDal, ScriptDTO, ScriptInfo>, IScript
    {
        #region Ctor

        private IProviderDal _providerDal;

        private IExplainPlanFactory _explainPlanFactory;

        private IColumnHeaderFactory _columnHeaderFactory;

        public Script(IScriptDal dal, IProviderDal providerDal, IMapper mapper, IExplainPlanFactory explainPlanFactory, IColumnHeaderFactory columnHeaderFactory) : base(dal, mapper)
        {
            _providerDal = providerDal;
            _explainPlanFactory = explainPlanFactory;
            _columnHeaderFactory = columnHeaderFactory;
        }

        #endregion

        #region Methods

        public async Task<List<ScriptDTO>> GetAllAsync()
        {
            var list = await _dal.GetAll();
            return _mapper.Map<List<ScriptInfo>, List<ScriptDTO>>(list);
        }

        public new async Task<int> InsertAsync(ScriptDTO dto)
        {
            bool success = await base.InsertAsync(dto);

            if (!success)
                throw new BusinessException("Não foi possível inserir o script");

            return dto.Id;
        }

        public override async Task<bool> UpdateAsync(ScriptDTO dto)
        {
            bool success = await base.UpdateAsync(dto);

            if (!success)
                throw new BusinessException("Não foi possível editar o script");

            return success;
        }

        public override async Task<bool> DeleteAsync(int id)
        {

            bool success = false;
            success = await base.DeleteAsync(id);

            if (!success)
                throw new BusinessException("Não foi possível deletar o script");

            return success;
        }

        public async Task<List<ScriptDTO>> GetByParamsAsync(int? id, int? providerId, string description, string script, string bandType, string processType, int page, int amountByPage)
        {
            var list = await _dal.GetByParamsAsync(id, providerId, description, script, bandType, processType, page, amountByPage);

            return _mapper.Map<List<ScriptInfo>, List<ScriptDTO>>(list);
        }

        public async Task<ExplainPlanDTO> GetExplainPlan(int providerId, string query)
        {
            var result = new ExplainPlanDTO()
            {
                Success = true
            };

            try
            {
                var provider = await _providerDal.GetByIdAsync(providerId);

                var explainPlan = _explainPlanFactory.CreateNew(provider.ProviderType, provider.ConnectionString, query);

                result.ExplainPlan = await explainPlan.getExplainPlan();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<List<ScriptDTO>> GetByProcessTypeAsync(string processType)
        {
            var list = await _dal.GetByProcessTypeAsync(processType);
            return _mapper.Map<List<ScriptInfo>, List<ScriptDTO>>(list);
        }

        public async Task<List<ScriptDTO>> GetByBandTypeAsync(string processType, string bandType)
        {
            var list = await _dal.GetByBandTypeAsync(processType, bandType);
            return _mapper.Map<List<ScriptInfo>, List<ScriptDTO>>(list);
        }

        public async Task<string[]> RetrieveColumns(int scriptId)
        {
            var script = await _dal.GetByIdAsync(scriptId);
            var provider = await _providerDal.GetByIdAsync(script.ProviderId);

            var oracleColumnHeaderHelper = _columnHeaderFactory.CreateNew(provider.ProviderType,provider.ConnectionString, script.Script, new List<Helpers.DTO.QueryParameter>());

            return oracleColumnHeaderHelper.GetColumnHeaders().Select(x => x.Name).ToArray();
        }

        public async Task<ScriptDTO> GetByIdAsync(int id)
        {
            var info = await _dal.GetByIdAsync(id);
            return _mapper.Map<ScriptInfo, ScriptDTO>(info);
        }

        #endregion
    }
}
