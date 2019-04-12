using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Model
{
    public class GenerateReturnSatusInfo : BaseInfo
    {
        public int Id { get; set; }

        public ProgressStatusEnum ProgressStatus { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Hour { get; set; }

        public GenerateReturnInfo GenerateReturn { get; set; } = new GenerateReturnInfo();
    }
}
