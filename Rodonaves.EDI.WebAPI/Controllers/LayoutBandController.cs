using Microsoft.AspNetCore.Mvc;
using RTEFramework.Security;
using RTEFramework.Web.Security.Infra;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [RTEAuthorize]
    [ApiController]
    [Route("api/layoutbands")]
    public class LayoutBandController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Layout";
        private readonly ICurrentUser _currentUser;

        #endregion

        #region Ctor

        public LayoutBandController(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        #endregion
    }
}
