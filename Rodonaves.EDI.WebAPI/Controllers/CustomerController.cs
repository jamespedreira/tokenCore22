using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.WebAPI.Requests;
using RTEFramework.BLL.Infra;
using RTEFramework.Security;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Clientes";
        
        private readonly ICustomer _customer;
        private readonly ICurrentUser _currentUser;

        #endregion

        #region Ctor

        public CustomerController(ICustomer customer, ICurrentUser currentUser)
        {
            _customer = customer;
            _currentUser = currentUser;
        }

        #endregion

        #region API

        /// <summary>Clientes</summary>
        /// <remarks>Retorna todos os Clientes</remarks>
        /// <returns>Lista de Clientes</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _customer.GetAllAsync(_currentUser.CompanyId);

                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Clientes</summary>
        /// <remarks>Retorna todos os Clientes compatíveis com os filtros informado</remarks>
        /// <returns>Lista de Clientes</returns>
        [HttpGet("search")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetByParams([FromQuery] CustomerGetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var dto = GetNewCustomerInstance(request);

                var list = await _customer.GetByParams(dto, request.page.GetValueOrDefault(), request.amountByPage.GetValueOrDefault());

                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Clientes</summary>
        /// <remarks>Retorna todos os Clientes filtrado por tipo de processo do arcodo</remarks>
        /// <returns>Lista de Clientes</returns>
        [HttpGet("searchByProcessType")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetByProcessType([FromQuery] CustomerGetByProcessTypeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var list = await _customer.GetByProcessType(_currentUser.CompanyId, request.ProcessType.GetValueOrDefault(), request.page.GetValueOrDefault(), request.amountByPage.GetValueOrDefault());

                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private CustomerDTO GetNewCustomerInstance(CustomerGetRequest request)
        {
            return new CustomerDTO()
            {
                CompanyId = _currentUser.CompanyId,
                Person = new PersonDTO()
                {
                    Description = request.Description,
                    TaxIdRegistration = request.TaxIdRegistration
                }
            };
        }

        #endregion

    }
}
