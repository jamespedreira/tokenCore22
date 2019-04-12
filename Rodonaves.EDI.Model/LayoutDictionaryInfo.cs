using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class LayoutDictionaryInfo : BaseInfo
    {

        public int Id { get; set; }

        public string TmsValue { get; set; }

        public string ReferenceValue { get; set; }

        public int LayoutColumnId { get; set; }
    }
}