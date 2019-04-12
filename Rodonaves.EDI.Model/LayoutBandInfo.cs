using System.Collections.Generic;
using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class LayoutBandInfo : BaseInfo
    {
        public LayoutBandInfo ()
        {
            LayoutColumns = new List<LayoutColumnInfo> ();
        }

        public long Id { get; set; }
        public int Sequence { get; set; }
        public string BandTypeId { get; set; }

        public int ScriptId { get; set; }
        public ScriptInfo Script { get; set; }

        public bool Break { get; set; }
        public int LayoutHeaderId { get; set; }
        public string KeyBand { get; set; }
        public string KeyParentBand { get; set; }
        public List<LayoutColumnInfo> LayoutColumns { get; set; }

        public bool Writed { get; set; }
    }
}