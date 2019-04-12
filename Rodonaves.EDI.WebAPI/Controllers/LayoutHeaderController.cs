using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.WebAPI.Requests;
using RTEFramework.BLL.Infra;
using RTEFramework.Security;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route("api/layoutheaders")]
    public class LayoutHeaderController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Layout";
        private readonly ILayoutHeader _layoutHeader;
        private readonly ICurrentUser _currentUser;

        #endregion

        #region Ctor

        public LayoutHeaderController(ICurrentUser currentUser, ILayoutHeader layoutHeader)
        {
            _layoutHeader = layoutHeader;
            _currentUser = currentUser;
        }

        #endregion

        /// <summary>Layout</summary>
        /// <remarks>Retorna todos os layouts</remarks>
        /// <returns>Lista de Layout</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _layoutHeader.GetAllAsync(_currentUser.CompanyId);

                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Layout</summary>
        /// <remarks>Retorna os layouts por parâmetros</remarks>
        /// <returns>Lista de Layout</returns>
        [HttpGet]
        [Route("search")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetByParams([FromQuery] LayoutHeaderRequest request)
        {
            try
            {
                var list = await _layoutHeader.GetByParamsAsync(
                    request.id.GetValueOrDefault(),
                    request.layoutGroupId,
                    request.description,
                    request.active,
                    request.scriptId,
                    _currentUser.CompanyId,
                    request.processType,
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

        /// <summary>Cadastro de Layout</summary>
        /// <remarks>Insere um novo layout</remarks>
        /// <param name="dto">Objeto que representa um layout</param>
        /// <returns>Retorna se a inserção foi realizada com sucesso</returns>
        [HttpPost]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> Insert([FromBody]LayoutHeaderDTO dto)
        {
            dto.CompanyId = _currentUser.CompanyId;
            try
            {
                int newId = 0;

                if (ModelState.IsValid == false)
                    return BadRequest(ModelState);

                newId = await _layoutHeader.InsertAsync(dto);

                return Ok(newId);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Atualização de Layout</summary>
        /// <remarks>Atualiza um layout cadastrado</remarks>
        /// <param name="dto">Objeto que representa um layout</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpPut]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> Update([FromBody]LayoutHeaderDTO dto)
        {
            dto.CompanyId = _currentUser.CompanyId;
            try
            {
                bool success = false;

                if (ModelState.IsValid == false)
                    return BadRequest(ModelState);

                success = await _layoutHeader.UpdateAsync(dto);

                if (success)
                    return Ok(dto.Id);

                return BadRequest(success);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Remoção de Layout</summary>
        /// <remarks>Remove um layout cadastrado</remarks>
        /// <param name="id">Identificador único do layout</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpDelete]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> Delete([FromQuery][Required] int id)
        {
            try
            {
                bool success = await _layoutHeader.DeleteAsync(id);

                return Ok(success);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Layout</summary>
        /// <remarks>Retorna todos os Layouts ifiltrados por tipo de processo do arcodo</remarks>
        /// <returns>Lista de Layout</returns>
        [HttpGet("searchByProcessType")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetByProcessType([FromQuery] LayoutHeaderGetByProcessTypeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var list = await _layoutHeader.GetByProcessType(_currentUser.CompanyId, request.ProcessType.GetValueOrDefault(), request.CustomerId.GetValueOrDefault(), request.page.GetValueOrDefault(), request.amountByPage.GetValueOrDefault());

                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
