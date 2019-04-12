using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Rodonaves.EDI.WebAPI.Requests
{
    /// <summary>
    /// Representa os parâmetros de busca do cliente por acordo
    /// </summary>
    public class CustomerGetByProcessTypeRequest : PagedRequest
    {
        /// <summary>
        /// Tipo de processo do acordo
        /// </summary>
        [FromQuery, Required]
        public int? ProcessType { get; set; }
    }
}
