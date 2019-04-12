using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Rodonaves.EDI.WebAPI.Requests
{
    public class PagedRequest
    {
        /// <summary>
        /// Página atual da requisição.
        /// </summary>
        [FromQuery]
        [Required]
        public int? page { get; set; }

        /// <summary>
        /// Quantidade de itens por Página (Max. 100).
        /// </summary>
        [FromQuery]
        [Required]
        public int? amountByPage { get; set; }
    }
}
