using Microsoft.AspNetCore.Mvc;

namespace Rodonaves.EDI.WebAPI.Requests
{
    public class DetailedStatusRequest : PagedRequest
    {
        /// <summary>
        /// Customer description
        /// </summary>
        [FromQuery]
        public string customer { get; set; }

        /// <summary>
        /// Document key
        /// </summary>
        [FromQuery]
        public string key { get; set; }

        /// <summary>
        /// Process
        /// </summary>
        [FromQuery]
        public string process { get; set; }
    }
}
