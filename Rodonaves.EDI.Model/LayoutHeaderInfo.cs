using System.Collections.Generic;
using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class LayoutHeaderInfo : BaseInfo
    {
        public long Id { get; set; }
        public long LayoutGroupId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public long ScriptId { get; set; }

        public ScriptInfo Script { get; set; }

        public long CompanyId { get; set; }
        public ProcessTypeEnum ProcessType { get; set; }
        public int? AmountPages { get; set; }
        public List<LayoutBandInfo> LayoutBands { get; set; }
        public List<LayoutFileNameInfo> LayoutFileNames { get; set; }

        public LayoutHeaderInfo ()
        {
            Active = true;
            LayoutBands = new List<LayoutBandInfo> ();
            LayoutFileNames = new List<LayoutFileNameInfo> ();
        }
    }
}