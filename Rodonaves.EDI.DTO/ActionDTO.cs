using System.Collections.Generic;

namespace Rodonaves.EDI.DTO
{
    public class ActionDTO : BaseDTO
    {
        public string Description { get; set; }
        public TaskDTO Task { get; set; }
        public List<ActionObjectDTO> Objects { get; set; }

    }
}