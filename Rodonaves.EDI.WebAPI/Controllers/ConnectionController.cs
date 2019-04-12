using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.WebAPI.Requests;
using RTEFramework.BLL.Infra;
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
    [Route("api/connection")]
    public class ConnectionController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Conexão";
        private readonly IConnection _connection;

        #endregion

        #region Ctor

        public ConnectionController(IConnection connection)
        {
            _connection = connection;
        }

        #endregion

        [HttpPost("testFtpConnection")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> TestFtpConnection([FromBody, Required] FTPRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _connection.TestFtpConnection(request.Address, request.SenderFolder, request.ReceiverFolder, request.User, request.Password, request.Port, request.ActiveMode, request.EnableSSH);

                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
