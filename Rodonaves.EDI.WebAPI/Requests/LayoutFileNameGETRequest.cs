using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Requests
{
    /// <summary>
    /// Representa o objeto de busca para o arquivo de layout
    /// </summary>
    public class LayoutFileNameGETRequest : PagedRequest
    {
        /// <summary>
        /// Identificado único
        /// </summary>
        [FromQuery]
        public int? Id { get; set; }

        /// <summary>
        /// Identificador de referência do Layout
        /// </summary>
        [FromQuery]
        public int? LayoutHeaderId { get; set; }

        /// <summary>
        /// Descrição
        /// </summary>
        [FromQuery]
        public string Description { get; set; }

        /// <summary>
        /// Padrão
        /// </summary>
        [FromQuery]
        public bool? Default { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [FromQuery]
        public bool? Active { get; set; }
    }
}
