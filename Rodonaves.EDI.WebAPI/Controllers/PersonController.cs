using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.WebAPI.Requests;
using Rodonaves.Engine;
using RTEFramework.BLL.Infra;
using RTEFramework.Security;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route("api/persons")]
    public class PersonController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Pessoa";
        private readonly IPerson _person;
        private readonly ICurrentUser _currentUser;

        private string personToken= null;

        #endregion

        #region Ctor

        public PersonController(ICurrentUser currentUser, IPerson person)
        {
            _person = person;
            _currentUser = currentUser;
        }

        #endregion

        #region API

        /// <summary>Pessoa</summary>
        /// <remarks>Retorna todos as Pessoas da EDI</remarks>
        /// <returns>Lista de Pessoas EDI</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _person.GetAllAsync(_currentUser.CompanyId);

                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Pessoa</summary>
        /// <remarks>Retorna todos as Pessoas API Externa</remarks>
        /// <returns>Lista de Pessoas API Externa</returns>
        [HttpGet]
        [Route("externalperson")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetPersonExternal([FromQuery] PersonRequest request)
        {
            try
            {
                var list = await _person.GetPersonExternal(request.page, request.amountByPage, request.Description, request.TaxIdRegistration);

                if (!string.IsNullOrEmpty(list.ToString()))
                    return Ok(list);
                else
                    throw new BusinessException("Não foi possivel acessar API de pessoas");
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex);
            }
        }

        #endregion

    }
}