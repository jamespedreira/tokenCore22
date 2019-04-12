using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Requests
{
    public class TestConnectionRequest
    {
        /// <summary>
        /// Conexão
        /// </summary>
        [FromBody, Required]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Provedor
        /// </summary>
        [FromBody, Required]
        public ProviderTypeEnum ProviderType { get; set; }
    }
}
