using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.WebAPI.Requests;
using RTEFramework.BLL.Infra;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;

namespace Rodonaves.EDI.WebAPI.Controllers
{
     /// <summary>
    /// API de Provedores.
    /// </summary>
    [RTEAuthorize]
    [ApiController]
    [Route ("api/providers")]
    public class ProviderController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Provedores";
        private readonly IProvider _providers;

        #endregion

        #region Ctor
        /// <summary>Construtor do controller de provedores</summary>
        public ProviderController (IProvider providers)
        {
            this._providers = providers;
        }

        #endregion

        /// <summary>Provedores</summary>
        /// <remarks>Retorna todos os provedores</remarks>
        /// <returns>Lista de Provedores</returns>
        [HttpGet]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> GetAll ()
        {
            try
            {
                var list = await _providers.GetAllAsync ();

                return Ok (list);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Provedores</summary>
        /// <remarks>Retorna o provedores por parâmetros</remarks>
        /// <returns>Provedor</returns>
        [HttpGet]
        [Route ("search")]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> GetByParams ([FromQuery] ProviderRequest request)
        {
            try
            {
                var list = await _providers.GetByParamsAsync (
                    request.id.GetValueOrDefault (),
                    request.providerType,
                    request.name,
                    request.description,
                    request.connecionString,
                    request.page.GetValueOrDefault (),
                    request.amountByPage.GetValueOrDefault ()
                );

                return Ok (list);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Cadastro de Provedores</summary>
        /// <remarks>Insere um novo provedor</remarks>
        /// <param name="dto">Objeto que representa um provedor</param>
        /// <returns>Retorna se a inserção foi realizada com sucesso</returns>
        [HttpPost]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> Insert ([FromBody] ProviderDTO dto)
        {
            try
            {
                int newId = 0;
                if (ModelState.IsValid)
                    newId = await _providers.InsertAsync (dto);

                return Ok (newId);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>
        /// Testa a string de conexão informada
        /// </summary>
        /// <returns>Retorna se a conexão foi realizada com sucesso</returns>
        [HttpPost ("testConnection")]
        public async Task<IActionResult> TestConnection ([FromBody] TestConnectionRequest obj)
        {
            try
            {
                return Ok (await _providers.TestConnection (obj.ConnectionString, obj.ProviderType));
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Atualização de Provedores</summary>
        /// <remarks>Atualiza um provedor cadastrado</remarks>
        /// <param name="dto">Objeto que representa um provedor</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpPut]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> Update ([FromBody] ProviderDTO dto)
        {
            try
            {
                bool success = false;
                if (ModelState.IsValid)
                    success = await _providers.UpdateAsync (dto);

                return Ok (dto.Id);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Remoção de Provedores</summary>
        /// <remarks>Remove um provedor cadastrado</remarks>
        /// <param name="id">Identificador único do provedor</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpDelete]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> Delete ([FromQuery][Required] int id)
        {
            try
            {
                bool success = await _providers.DeleteAsync (id);

                return Ok (success);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }
    }
}