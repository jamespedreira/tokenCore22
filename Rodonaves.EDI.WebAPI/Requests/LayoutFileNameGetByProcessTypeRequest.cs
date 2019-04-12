using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Rodonaves.EDI.WebAPI.Requests
{
    /// <summary>
    /// Representa os parâmetros de busca do cliente por acordo
    /// </summary>
    public class LayoutFileNameGetByProcessTypeRequest : PagedRequest
    {
        /// <summary>
        /// Tipo de processo do acordo
        /// </summary>
        [FromQuery, Required]
        public int? ProcessType { get; set; }

        /// <summary>
        /// Cliente do acordo
        /// </summary>
        [FromQuery, Required]
        public int? CustomerId { get; set; }
    }
}
