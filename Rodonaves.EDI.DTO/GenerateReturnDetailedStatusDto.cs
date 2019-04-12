using System;
using System.Collections.Generic;
using Rodonaves.EDI.Enums;

namespace Rodonaves.EDI.DTO
{
    public class GenerateReturnDetailedStatusDTO : BaseDTO
    {
        public string Customer { get; set; }

        public ProcessTypeEnum ProcessType { get; set; }

        public ProgressStatusEnum ProgressStatus { get; set; }

        public int TotalFreights { get; set; }

        public TimeSpan StartingTime { get; set; }

        public TimeSpan EndingTime { get; set; }

        public DateTime Date { get; set; }

        public long GenerateReturnId { get; set; }

        public List<LogDTO> Logs { get; set; }

        public int TotalItems { get; set; }

        public string Key { get; set; }

        public TimeSpan TimeStampProcess
        {
            get
            {
                TimeSpan timeDiff;
                if (EndingTime < StartingTime)
                {
                    var datWithTime = Date.AddHours (StartingTime.Hours);
                    datWithTime = datWithTime.AddMinutes (StartingTime.Minutes);
                    datWithTime = datWithTime.AddSeconds (StartingTime.Seconds);

                    long ticksDiff = DateTime.Now.Ticks - datWithTime.Ticks;
                    timeDiff = new TimeSpan (ticksDiff);
                    timeDiff = new TimeSpan (timeDiff.Hours + (timeDiff.Days * 24), timeDiff.Minutes, timeDiff.Seconds);
                }
                else
                {
                    timeDiff = EndingTime - StartingTime;
                }
                return timeDiff;
            }
            set { }
        }
    }
}