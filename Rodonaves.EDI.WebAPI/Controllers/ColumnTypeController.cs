using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rodonaves.Core.Bll;
using Rodonaves.EDI.Enums;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route ("api/columntype")]
    public class ColumnTypeController : ControllerBase
    {
        #region Properties

        const string _controllerName = "ColumnType";

        private readonly Parameter _parameter;

        #endregion

        public ColumnTypeController (Parameter parameter)
        {
            _parameter = parameter;
        }

        /// <summary>Retorna Opção de colunas</summary>
        /// <remarks>Retorna os tipos de colunas que se pode selecionar</remarks>
        /// <returns>Tipos de colunas</returns>
        [HttpGet]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public IActionResult GetColumnTypes (string processType)
        {
            return Ok (EDIEnumsList.GetColumnType ());
        }

        /// <summary>Retorna Opção de formato da coluna</summary>
        /// <remarks>Retorna os opções de formato da coluna</remarks>
        /// <returns>Formato da coluna</returns>
        [HttpGet ("format")]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public IActionResult GetFormatTypes ()
        {
            var parameterInfo = _parameter.GetParameter ("FMTLAY");

            return Ok (parameterInfo.Items.Select (x => new
            {
                Value = x.Value.ToString(),
                Description = x.Description.ToString()
            }));
        }
    }
}