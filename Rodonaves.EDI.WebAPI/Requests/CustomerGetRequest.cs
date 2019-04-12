using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Requests
{
    /// <summary>
    /// Classe que representa os parâmetros de busca do cliente
    /// </summary>
    public class CustomerGetRequest : PagedRequest
    {
        /// <summary>
        /// Descrição do Cliente
        /// </summary>
        [FromQuery]
        public string Description { get; set; }

        /// <summary>
        /// CPF ou CNPJ do cliente
        /// </summary>
        [FromQuery]
        public string TaxIdRegistration { get; set; }

    }
}
