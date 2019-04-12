using System;
using System.Collections.Generic;
using System.Text;
using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class TaskInfo : BaseInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TriggerInfo> Triggers { get; set; }
        public List<ActionInfo> Actions { get; set; }
        public int? AmountPages { get; set; }
    }
}