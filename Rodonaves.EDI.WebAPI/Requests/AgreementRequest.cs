using Microsoft.AspNetCore.Mvc;

namespace Rodonaves.EDI.WebAPI.Requests
{
    /// <summary>
    /// Representa os parâmetros de busco do acordo
    /// </summary>
    public class AgreementRequest : PagedRequest
    {
        /// <summary>
        /// Identificador do acordo
        /// </summary>
        [FromQuery]
        public int? Id { get; set; }

        /// <summary>
        /// Identificado do cliente
        /// </summary>
        [FromQuery]
        public int? CustomerId { get; set; }

        /// <summary>
        /// Descrição do cliente
        /// </summary>
        [FromQuery]
        public string CustomerDescription { get; set; }

        /// <summary>
        /// CPF/CNPJ do cliente
        /// </summary>
        [FromQuery]
        public string CustomerTaxIdRegistration { get; set; }

        /// <summary>
        /// Status do acordo
        /// </summary>
        [FromQuery]
        public bool? Active { get; set; }
    }
}
