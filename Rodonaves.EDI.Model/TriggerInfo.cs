using System;
using System.Collections.Generic;
using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class TriggerInfo : BaseInfo
    {
        public TriggerInfo ()
        {
            Task = new TaskInfo ();
        }
        public int Id { get; set; }

        public TaskInfo Task { get; set; }

        public FrequencyType Frequency { get; set; }

        public int Interval { get; set; }

        public DateTime BeginDate { get; set; }

        public TimeSpan BeginTime { get; set; }

        public DateTime ExpireDate { get; set; }

        public TimeSpan ExpireTime { get; set; }

        public bool Sunday { get; set; }

        public bool Monday { get; set; }

        public bool Tuesday { get; set; }

        public bool Wednesday { get; set; }

        public bool Thursday { get; set; }

        public bool Friday { get; set; }

        public bool Saturday { get; set; }

        public bool Enable { get; set; }

        public List<int> DaysOfMonth { get; set; }

        public List<Month> Months { get; set; }

        public int? AmountPages { get; set; }
    }
}