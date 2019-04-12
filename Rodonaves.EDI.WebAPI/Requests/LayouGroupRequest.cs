using Microsoft.AspNetCore.Mvc;

namespace Rodonaves.EDI.WebAPI.Requests
{
    public class LayoutGroupRequest : PagedRequest
    {
        /// <summary>
        /// Identificador único do grupo de layout
        /// </summary>
        [FromQuery]
        public int? id { get; set; }

        /// <summary>
        /// Descrição do grupo de layout
        /// </summary>
        [FromQuery]
        public string description { get; set; }

        /// <summary>
        /// Empresa desenvolvedora do grupo de layout
        /// </summary>
        [FromQuery]
        public string developmentCompany { get; set; }
    }
}
