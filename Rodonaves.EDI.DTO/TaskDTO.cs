using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.DTO
{
    public class TaskDTO : BaseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TriggerDTO> Triggers { get; set; }
        public List<ActionDTO> Actions { get; set; }
    }
}