using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Requests
{
    public class ScriptRequest : PagedRequest
    {
        /// <summary>
        /// Identificador único do provedor
        /// </summary>
        [FromQuery]
        public int? Id { get; set; }

        /// <summary>
        /// Tipo do provedor
        /// </summary>
        [FromQuery]
        public int? ProviderId { get; set; }

        /// <summary>
        /// Descrição
        /// </summary>
        [FromQuery]
        public string Description { get; set; }

        /// <summary>
        /// Script
        /// </summary>
        [FromQuery]
        public string Script { get; set; }

        /// <summary>
        /// Tipo de banda
        /// </summary>
        [FromQuery]
        public string BandType { get; set; }

        /// <summary>
        /// Processo
        /// </summary>
        [FromQuery]
        public string ProcessType { get; set; }
    }
}
