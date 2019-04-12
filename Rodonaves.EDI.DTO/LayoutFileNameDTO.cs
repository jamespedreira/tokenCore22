using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.DTO
{
    public class LayoutFileNameDTO : BaseDTO
    {
        public LayoutFileNameDTO()
        {
            Active = true;
            LayoutColumns = new List<LayoutColumnDTO>();
        }

        public int? LayoutHeaderId { get; set; }

        public string Description { get; set; }

        public bool Default { get; set; }

        public bool Active { get; set; }

        public List<LayoutColumnDTO> LayoutColumns { get; set; }
    }
}
