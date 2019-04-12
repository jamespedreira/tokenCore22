using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Model
{
    public class LayoutFileNameInfo : BaseInfo
    {
        public LayoutFileNameInfo()
        {
            Active = true;
            LayoutColumns = new List<LayoutColumnInfo>();
        }

        public int Id { get; set; }

        public int LayoutHeaderId { get; set; }

        public string Description { get; set; }

        public bool Default { get; set; }

        public bool Active { get; set; }

        public int AmountPages { get; set; }

        public List<LayoutColumnInfo> LayoutColumns { get; set; }
    }
}
