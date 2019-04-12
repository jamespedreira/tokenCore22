using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using RTEFramework.BLL.Infra;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    /// <summary>
    /// Geração de retorno
    /// </summary>
    [RTEAuthorize]
    [ApiController]
    [Route("api/generatereturn")]
    public class GenerateReturnController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Gerar Retorno";

        private readonly IGenerateReturn _generateReturn;

        #endregion

        #region Ctor

        public GenerateReturnController(IGenerateReturn generateReturn)
        {
            _generateReturn = generateReturn;
        }

        #endregion

        /// <summary>Geração de retorno</summary>
        /// <remarks>Retorna todos os gerações de retorno</remarks>
        /// <returns>Lista de gerações de retorno</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _generateReturn.GetAllAsync();

                return Ok(list);
            }
            catch (BusinessException ex)
            {

                return BadRequest(ex.Message);
            }
        }

        /// <summary>Geração de retorno</summary>
        /// <remarks>Insere uma nova geração de retorno</remarks>
        /// <returns>Identificador único do retorno</returns>
        [HttpPost]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> Insert([FromBody, Required] GenerateReturnDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var id = await _generateReturn.InsertAsync(dto);

                return Ok(id);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
