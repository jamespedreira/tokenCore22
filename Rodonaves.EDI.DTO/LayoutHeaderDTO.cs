using System.Collections.Generic;
using Rodonaves.EDI.Enums;

namespace Rodonaves.EDI.DTO
{
    public class LayoutHeaderDTO : BaseDTO
    {
        public long LayoutGroupId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int ScriptId { get; set; }

        public ScriptDTO Script { get; set; }

        public long CompanyId { get; set; }
        public ProcessTypeEnum ProcessType { get; set; }
        public List<LayoutBandDTO> LayoutBands { get; set; }
        public List<LayoutFileNameDTO> LayoutFileNames { get; set; }

        public LayoutHeaderDTO ()
        {
            Active = true;
            LayoutBands = new List<LayoutBandDTO> ();
            LayoutFileNames = new List<LayoutFileNameDTO> ();
        }
    }
}