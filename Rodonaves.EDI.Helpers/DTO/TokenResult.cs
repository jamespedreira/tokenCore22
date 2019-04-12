using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Helpers.DTO
{
    public class TokenResult
    {
        /// <summary>
        /// Token Identificador
        /// </summary>
        public string Access_token { get; set; }

        /// <summary>
        /// Tipo do Token Identificador
        /// </summary>
        public string Token_type { get; set; }

        /// <summary>
        /// ID do usuário
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Nome do usuário
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ID da Empresa
        /// </summary>
        public int CompanyId { get; set; }
    }
}
