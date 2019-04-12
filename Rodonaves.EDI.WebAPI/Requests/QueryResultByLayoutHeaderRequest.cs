using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Requests
{
    public class QueryResultByLayoutHeaderRequest
    {
        public int LayoutHeaderId { get; set; }

        public int CustomerId { get; set; }

        public DateTime StartingDate { get; set; }

        public DateTime EndingDate { get; set; }

        public int? BillOfLadingId { get; set; }
    }
}
