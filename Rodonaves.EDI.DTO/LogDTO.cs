using System;

namespace Rodonaves.EDI.DTO
{
    public class LogDTO : BaseDTO
    {
        public int GenerateReturnId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int Type { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Hour { get; set; }
    }
}
