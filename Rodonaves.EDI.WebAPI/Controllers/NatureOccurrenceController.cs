using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route("api/natureoccurrences")]
    public class NatureOccurrenceController : Controller
    {

        #region Properties

        const string _controllerName = "Natureza de Ocorrência";
        private readonly ICurrentUser _currentUser;
        private readonly INatureOccurrence _natureOccurrence;

        #endregion

        #region Ctor

        public NatureOccurrenceController(ICurrentUser currentUser, INatureOccurrence NatureOccurrence)
        {
            _currentUser = currentUser;
            _natureOccurrence = NatureOccurrence;
        }

        #endregion

        #region API
        /// <summary>Retorna Naturezas de Ocorrência</summary>
        /// <remarks>Retorna os tipos de Naturezas de Ocorrência</remarks>
        /// <returns>Natureza de Ocorrências</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetNatureOccurrences()
        {
            try
            {
                var list = await _natureOccurrence.GetNatureOccurrences();

                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}