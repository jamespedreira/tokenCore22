using Microsoft.AspNetCore.Mvc;

namespace Rodonaves.EDI.WebAPI.Requests
{
    public class ProviderRequest : PagedRequest
    {
        /// <summary>
        /// Identificador único do provedor
        /// </summary>
        [FromQuery]
        public int? id { get; set; }

        /// <summary>
        /// Tipo do provedor
        /// </summary>
        [FromQuery]
        public int? providerType { get; set; }

        /// <summary>
        /// Conexão
        /// </summary>
        [FromQuery]
        public string connecionString { get; set; }

        /// <summary>
        /// Descrição do provedor
        /// </summary>
        [FromQuery]
        public string description { get; set; }

        /// <summary>
        /// Nome do provedor
        /// </summary>
        [FromQuery]
        public string name { get; set; }
    }
}
