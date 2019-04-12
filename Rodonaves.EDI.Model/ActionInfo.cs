using System.Collections.Generic;
using Rodonaves.Core.Model;
using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class ActionInfo : BaseInfo
    {
        public ActionInfo ()
        {
            Task = new TaskInfo ();
        }
        public int Id { get; set; }
        public string Description { get; set; }
        public TaskInfo Task { get; set; }
        public List<ActionObjectInfo> Objects { get; set; }
    }
}