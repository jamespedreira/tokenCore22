using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rodonaves.Core.Bll;
using Rodonaves.EDI.BLL.Interfaces;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route ("api/bandtypes")]
    public class BandTypeController : ControllerBase
    {
        #region Properties

        const string _controllerName = "BandType";
        private readonly IBandType _bandType;

        #endregion

        public BandTypeController (IBandType bandType)
        {
            _bandType = bandType;
        }

        /// <summary>Retorna Opção de bandas</summary>
        /// <remarks>Retorna os opções de banda</remarks>
        /// <returns>Tipos de banda</returns>
        [HttpGet]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> GetBandTypes (string processType)
        {
            try
            {
                var list = await _bandType.GetBandTypes(processType);

                return Ok(list);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}