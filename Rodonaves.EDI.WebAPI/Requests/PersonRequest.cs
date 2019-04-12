using Microsoft.AspNetCore.Mvc;

namespace Rodonaves.EDI.WebAPI.Requests
{
    public class PersonRequest : PagedRequest
    {
        /// <summary>
        /// Descrição do Nome da Pessoa
        /// </summary>
        [FromQuery]
        public string Description { get; set; }

        /// <summary>
        /// CPF/CNPJ da pessoa
        /// </summary>
        [FromQuery]
        public string TaxIdRegistration { get; set; }

      

    }
}
