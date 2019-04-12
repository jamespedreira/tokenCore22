using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.Enums;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route("api/processtype")]
    public class ProcessTypeController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Tipos de Processo";

        #endregion

        /// <summary>Retorna Processos</summary>
        /// <remarks>Retorna os tipos de processos que se pode selecionar</remarks>
        /// <returns>Tipos de processos</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public IActionResult GetProcesses()
        {
            return Ok(EDIEnumsList.GetProcessType());
        }
    }
}
