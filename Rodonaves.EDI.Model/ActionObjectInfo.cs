using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class ActionObjectInfo : BaseInfo
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public int ObjectId { get; set; }
        public string Arguments { get; set; }
    }
}