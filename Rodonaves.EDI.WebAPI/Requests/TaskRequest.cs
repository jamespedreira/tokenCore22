using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Rodonaves.EDI.WebAPI.Requests
{
    /// <summary>
    /// TaskRequest
    /// </summary>
    public class TaskRequest : PagedRequest
    {
        /// <summary>
        /// Identificador único da tarefa
        /// </summary>
        [FromQuery]
        public int? id { get; set; }

        /// <summary>
        /// Descrição da tarefa
        /// </summary>
        [FromQuery]
        public string description { get; set; }

        /// <summary>
        /// Nome da tarefa
        /// </summary>
        [FromQuery]
        public string name { get; set; }
    }
}