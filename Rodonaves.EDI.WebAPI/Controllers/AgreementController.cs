using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.WebAPI.Requests;
using RTEFramework.BLL.Infra;
using RTEFramework.Security;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    //[RTEAuthorize]
    [ApiController]
    [Route("api/agreements")]
    public class AgreementController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Convênio de Cliente";
        private readonly IAgreement _agreement;
        private readonly ICustomer _customer;
        private readonly ICurrentUser _currentUser;

        #endregion

        #region Ctor
        
        public AgreementController(ICurrentUser currentUser, IAgreement agreement, ICustomer customer)
        {
            _agreement = agreement;
            _currentUser = currentUser;
            _customer = customer;
        }

        #endregion

        #region API - Routes

        /// <summary> Convênio </summary>
        /// <remarks>Retorna todos os Convênios</remarks>
        /// <returns>Lista de Convênios</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _agreement.GetAllAsync(_currentUser.CompanyId);

                return Ok(list);
            }
            catch (BusinessException ex)
            {

                return BadRequest(ex.Message);
            }
        }

        /// <summary>Convênio</summary>
        /// <remarks>Retorna os convênios por parâmetros</remarks>
        /// <returns>Lista de Convênio</returns>
        [HttpGet]
        [Route("search")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetByParams([FromQuery] AgreementRequest request)
        {
            try
            {
                var obj = GetNewAgreementInstance(request);

                var list = await _agreement.GetByParamsAsync(obj,
                    request.Active,
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
        
        /// <summary>Cadastro de convênio</summary>
        /// <remarks>Insere um novo convênio</remarks>
        /// <param name="dto">Objeto que representa um convênio</param>
        /// <returns>Retorna se a inserção foi realizada com sucesso</returns>
        [HttpPost]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> Insert([FromBody] AgreementDTO dto)
        {
            dto.CompanyId = _currentUser.CompanyId;
            dto.Customer.CompanyId = dto.CompanyId;           

            try
            {
                int existCustomerId = _customer.ExistCustomerId(dto.Customer.Person.TaxIdRegistration, dto.CompanyId);

                if (existCustomerId == 0)
                {
                    var customerId = await _customer.InsertAsync(dto.Customer);
                    dto.Customer.Id = customerId;
                }
                else
                { dto.Customer.Id = existCustomerId; }
                
                int newId = 0;

                if (ModelState.IsValid == false)
                    return BadRequest(ModelState);

                newId = await _agreement.InsertAsync(dto);

                return Ok(newId);
            }
            catch (BusinessException ex)
            {

                return BadRequest(ex.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>Atualização de convênio</summary>
        /// <remarks>Atualiza um convênio cadastrado</remarks>
        /// <param name="dto">Objeto que representa um convênio</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpPut]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> Update([FromBody]AgreementDTO dto)
        {
            dto.CompanyId = _currentUser.CompanyId;
            try
            {
                bool sucess = false;

                if (ModelState.IsValid == false)
                    return BadRequest(ModelState);

                sucess = await _agreement.UpdateAsync(dto);

                return Ok(dto.Id);
            }
            catch (BusinessException ex)
            {

                return BadRequest(ex.Message);
            }
        }

        /// <summary>Remoção de convênio</summary>
        /// <remarks>Remove um convênio cadastrado</remarks>
        /// <param name="id">Identificador único do convênio</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpDelete]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> Delete([FromQuery][Required] int id)
        {
            try
            {
                bool success = await _agreement.DeleteAsync(id);

                return Ok(success);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private AgreementDTO GetNewAgreementInstance(AgreementRequest request)
        {
            return new AgreementDTO()
            {
                CompanyId = _currentUser.CompanyId,
                Id = request.Id.GetValueOrDefault(),
                Active = request.Active.GetValueOrDefault(),
                Customer = new CustomerDTO()
                {
                    Id = request.CustomerId.GetValueOrDefault(),
                    CompanyId = _currentUser.CompanyId,
                    Person = new PersonDTO()
                    {
                        Description = request.CustomerDescription,
                        TaxIdRegistration = request.CustomerTaxIdRegistration
                    }
                }
            };
        }
        #endregion
    }
}