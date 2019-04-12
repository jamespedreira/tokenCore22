using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.Enums;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route("api/periodicitytype")]
    public class PeriodicityTypeController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Tipos de Periodicidade";

        #endregion

        /// <summary>Retorna Periodicidades</summary>
        /// <remarks>Retorna os tipos de periodicidade que se pode selecionar</remarks>
        /// <returns>Tipos de periodicidades</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public IActionResult GetPeriodicities()
        {
            return Ok(EDIEnumsList.GetPeriodicityType());
        }
    }
}