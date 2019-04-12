using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.WebAPI.Requests;
using RTEFramework.BLL.Infra;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route("api/layoutgroups")]
    public class LayoutGroupController : ControllerBase
    {
        #region Properties
    
        const string _controllerName = "Grupo de Layout";
        private readonly ILayoutGroup _layoutGroup;

        #endregion

        #region Ctor

        public LayoutGroupController(ILayoutGroup layoutGroup)
        {
            _layoutGroup = layoutGroup;
        }

        #endregion

        /// <summary>Grupo de Layout</summary>
        /// <remarks>Retorna todos os grupos de layout</remarks>
        /// <returns>Lista de Grupos de Layout</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _layoutGroup.GetAllAsync();

                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Grupo de Layout</summary>
        /// <remarks>Retorna os grupos de layout por parâmetros</remarks>
        /// <returns>Lista de Grupos de Layout</returns>
        [HttpGet]
        [Route("search")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetByParams([FromQuery] LayoutGroupRequest request)
        {
            try
            {
                var list = await _layoutGroup.GetByParamsAsync(
                    request.id.GetValueOrDefault(),
                    request.description,
                    request.developmentCompany,
                    request.page.GetValueOrDefault(),
                    request.amountByPage.GetValueOrDefault()
                );

                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Cadastro de Grupo de Layout</summary>
        /// <remarks>Insere um novo grupo de layout</remarks>
        /// <param name="dto">Objeto que representa um grupo de layout</param>
        /// <returns>Retorna se a inserção foi realizada com sucesso</returns>
        [HttpPost]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> Insert([FromBody]LayoutGroupDTO dto)
        {
            try
            {
                int newId = 0;

                if (ModelState.IsValid == false)
                    return BadRequest(ModelState);

                newId = await _layoutGroup.InsertAsync(dto);

                return Ok(newId);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Atualização de Grupo de Layout</summary>
        /// <remarks>Atualiza um grupo de layout cadastrado</remarks>
        /// <param name="dto">Objeto que representa um grupo de layout</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpPut]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> Update([FromBody]LayoutGroupDTO dto)
        {
            try
            {
                bool success = false;
                if (ModelState.IsValid == false)
                    return BadRequest(ModelState);

                success = await _layoutGroup.UpdateAsync(dto);

                return Ok(dto.Id);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Remoção de Grupo de Layout</summary>
        /// <remarks>Remove um grupo de layout cadastrado</remarks>
        /// <param name="id">Identificador único do grupo de layout</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpDelete]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> Delete([FromQuery][Required] int id)
        {
            try
            {
                bool success = await _layoutGroup.DeleteAsync(id);

                return Ok(success);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
