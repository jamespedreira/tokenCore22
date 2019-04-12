using System.Collections.Generic;

namespace Rodonaves.EDI.DTO
{
    public class LayoutBandDTO : BaseDTO
    {
        public LayoutBandDTO ()
        {
            LayoutColumns = new List<LayoutColumnDTO> ();
        }

        public int Sequence { get; set; }
        public string BandTypeId { get; set; }

        public int ScriptId { get; set; }
        public ScriptDTO Script { get; set; }

        public bool Break { get; set; }
        public int LayoutHeaderId { get; set; }
        public string KeyBand { get; set; }
        public string KeyParentBand { get; set; }
        public List<LayoutColumnDTO> LayoutColumns { get; set; }
    }
}