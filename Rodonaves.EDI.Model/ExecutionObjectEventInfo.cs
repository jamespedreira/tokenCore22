using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class ExecutionObjectEventInfo : BaseInfo
    {
        public int Id { get; set; }

        public int ExecutionObjectId { get; set; }

        public int Code { get; set; }

        public string CodeDescription { get; set; }

        public string Comment { get; set; }
    }
}
