using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rodonaves.Core.Bll;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.WebAPI.Requests;
using RTEFramework.BLL.Infra;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    /// <summary>
    /// API de Scripts.
    /// </summary>
    [RTEAuthorize]
    [ApiController]
    [Route ("api/scripts")]
    public class ScriptController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Scripts";
        private IScript _script;

        #endregion

        #region Ctor
        /// <summary>Contrutor do controller scripts</summary>
        public ScriptController (IScript script)
        {
            _script = script;
        }

        #endregion

        /// <summary>Scripts</summary>
        /// <remarks>Retorna todos os scripts</remarks>
        /// <returns>Lista de Scripts</returns>
        [HttpGet]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> GetAll ()
        {
            try
            {
                var list = await _script.GetAllAsync ();

                return Ok (list);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Scripts</summary>
        /// <remarks>Retorna os scripts por parâmetros</remarks>
        /// <returns>Lista de Scripts</returns>
        [HttpGet]
        [Route ("search")]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> GetByParams ([FromQuery] ScriptRequest request)
        {
            try
            {
                var list = await _script.GetByParamsAsync (
                    request.Id,
                    request.ProviderId,
                    request.Description,
                    request.Script,
                    request.BandType,
                    request.ProcessType,
                    request.page.GetValueOrDefault (),
                    request.amountByPage.GetValueOrDefault ()
                );

                return Ok (list);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest (ex);
            }
        }

        [HttpGet]
        [Route("byprocesstype")]
        [SwaggerOperation(Tags = new [] { _controllerName })]
        public async Task<IActionResult> GetByProcessType([FromQuery]string processType)
        {
            return Ok(await _script.GetByProcessTypeAsync(processType));
        }

        [HttpGet]
        [Route("bybandtype")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetByBandType([FromQuery]string processType, [FromQuery]string bandType)
        {
            return Ok(await _script.GetByBandTypeAsync(processType, bandType));
        }

        /// <summary>Cadastro de Scripts</summary>
        /// <remarks>Insere um novo script</remarks>
        /// <param name="dto">Objeto que representa um script</param>
        /// <returns>Retorna se a inserção foi realizada com sucesso</returns>
        [HttpPost]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> Insert ([FromBody] ScriptDTO dto)
        {
            try
            {
                int newId = 0;

                if (ModelState.IsValid == false)
                    return BadRequest (ModelState);

                newId = await _script.InsertAsync (dto);

                return Ok (newId);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Atualização de Script</summary>
        /// <remarks>Atualiza um script cadastrado</remarks>
        /// <param name="dto">Objeto que representa um script</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpPut]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> Update ([FromBody] ScriptDTO dto)
        {
            try
            {
                bool success = false;
                if (ModelState.IsValid)
                    success = await _script.UpdateAsync (dto);

                return Ok (dto.Id);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Remoção de Scripts</summary>
        /// <remarks>Remove um script cadastrado</remarks>
        /// <param name="id">Identificador único do script</param>
        /// <returns>Retorna se a remoção foi realizada com sucesso</returns>
        [HttpDelete]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> Delete ([FromQuery][Required] int id)
        {
            try
            {
                bool success = await _script.DeleteAsync (id);

                return Ok (success);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }
        
        /// <summary>Retorna o plano de execução do script</summary>
        /// <returns>Plano de execução</returns>
        [HttpPost]
        [Route ("explainplan")]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> GetExplainPlan ([FromBody] QueryRequest request)
        {
            try
            {
                var plan = await _script.GetExplainPlan (request.ProviderId, request.Query);

                return Ok (plan);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        [HttpGet]
        [Route("retrievecolumns")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> RetrieveColumns([FromQuery] int scriptId)
        {
            try
            {
                var columns = await _script.RetrieveColumns(scriptId);

                return Ok(columns);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}