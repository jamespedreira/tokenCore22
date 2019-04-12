using System;
using System.Collections.Generic;
using System.Text;
using Rodonaves.EDI.Enums;

namespace Rodonaves.EDI.DTO
{
    public class TriggerDTO : BaseDTO
    {
        public FrequencyType? Frequency { get; set; }

        public int? Interval { get; set; }

        public DateTime? BeginDate { get; set; }

        public TimeSpan? BeginTime { get; set; }

        public DateTime? ExpireDate { get; set; }

        public TimeSpan? ExpireTime { get; set; }

        public bool? Sunday { get; set; }

        public bool? Monday { get; set; }

        public bool? Tuesday { get; set; }

        public bool? Wednesday { get; set; }

        public bool? Thursday { get; set; }

        public bool? Friday { get; set; }

        public bool? Saturday { get; set; }

        public bool? Enable { get; set; }

        public int?[] DaysOfMonth { get; set; }

        public Month?[] Months { get; set; }

    }
}