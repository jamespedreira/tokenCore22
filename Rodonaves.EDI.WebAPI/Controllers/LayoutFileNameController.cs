using Microsoft.AspNetCore.Mvc;
using Rodonaves.Core.Bll;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Helpers;
using Rodonaves.EDI.WebAPI.Requests;
using RTEFramework.BLL.Infra;
using RTEFramework.Security;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route("api/layoutfilename")]
    public class LayoutFileNameController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Arquivo de layout";

        private readonly ILayoutFileName _layoutFileName;

        private readonly ICurrentUser _currentUser;

        private readonly Parameter _parameter;

        private readonly IScript _script;

        #endregion

        #region Ctor

        public LayoutFileNameController(ILayoutFileName layoutFileName, ICurrentUser currentUser, Parameter parameter, IScript script)
        {
            _layoutFileName = layoutFileName;
            _currentUser = currentUser;
            _parameter = parameter;
            _script = script;
        }

        #endregion

        #region Methods

        /// <summary> Arquivo de layout </summary>
        /// <remarks>Retorna todos os arquivos de layout</remarks>
        /// <returns>Lista de arquivos do layout</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _layoutFileName.GetAllAsync();

                return Ok(list);
            }
            catch (BusinessException ex)
            {

                return BadRequest(ex.Message);
            }
        }


        /// <summary> Arquivo de layout </summary>
        /// <remarks>Retorna todos os arquivos de layout</remarks>
        /// <returns>Lista de arquivos do layout</returns>
        [HttpGet("search")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetByParams([FromQuery] LayoutFileNameGETRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var obj = GetNewLayoutFileNameInstance(request);

                var list = await _layoutFileName.GetByParams(obj, request.page.GetValueOrDefault(), request.amountByPage.GetValueOrDefault());

                return Ok(list);
            }
            catch (BusinessException ex)
            {

                return BadRequest(ex.Message);
            }
        }

        /// <summary>Arquivo de layout</summary>
        /// <remarks>Retorna todos os arquivos de layout filtrado pelo tipo de processo do arcodo</remarks>
        /// <returns>Lista de arquivos do layout</returns>
        [HttpGet("searchByProcessType")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetByProcessType([FromQuery] LayoutFileNameGetByProcessTypeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var list = await _layoutFileName.GetByProcessType(_currentUser.CompanyId, request.ProcessType.GetValueOrDefault(), request.CustomerId.GetValueOrDefault(), request.page.GetValueOrDefault(), request.amountByPage.GetValueOrDefault());

                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>Retorna as colunas do script</summary>
        /// <remarks>Retorna as colunas do script</remarks>
        /// <returns>colunas do script</returns>
        [HttpPost("columns")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetScriptColumnsAsync(List<LayoutBandDTO> bandDTOs)
        {
            try
            {
                if (bandDTOs.Count <= 0)
                    return Ok();

                var band = bandDTOs.Find(b => b.BandTypeId.Equals(MainBandHelper.GetBandMain(bandDTOs, _parameter)));

                if (band != null)
                {
                    var scripts = await _script.RetrieveColumns(band.ScriptId);
                    return Ok(scripts);
                }

                return Ok();

            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion

        private LayoutFileNameDTO GetNewLayoutFileNameInstance(LayoutFileNameGETRequest request)
        {
            return new LayoutFileNameDTO()
            {
                Id = request.Id.GetValueOrDefault(),
                Active = request.Active.GetValueOrDefault(),
                Default = request.Default.GetValueOrDefault(),
                Description = request.Description,
                LayoutHeaderId = request.LayoutHeaderId.GetValueOrDefault()
            };
        }
    }
}
