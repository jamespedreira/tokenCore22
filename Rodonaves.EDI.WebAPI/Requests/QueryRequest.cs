using System.ComponentModel.DataAnnotations;

namespace Rodonaves.EDI.WebAPI.Requests
{
    public class QueryRequest
    {
        /// <summary>
        /// Identificados único do provedor
        /// </summary>
        [Required]
        public int ProviderId { get; set; }

        /// <summary>
        /// Query
        /// </summary>
        [Required]
        public string Query { get; set; }
    }
}
