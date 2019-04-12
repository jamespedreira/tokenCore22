using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;

namespace Rodonaves.EDI.Model
{
    public class GenerateReturnDetailedStatusInfo : BaseInfo
    {
        public string Customer { get; set; }

        public ProcessTypeEnum ProcessType { get; set; }

        public int TotalFreights { get; set; }

        public ProgressStatusEnum ProgressStatus { get; set; }

        public TimeSpan StartingTime { get; set; }

        public TimeSpan EndingTime { get; set; }

        public DateTime Date { get; set; }

        public long GenerateReturnId { get; set; }

        public List<LogInfo> Logs { get; set; }

        public int AmountPages { get; set; }

        public int TotalItems { get; set; }

        public string Key { get; set; }
    }
}
