using Rodonaves.Engine.BaseObjects;
using System;

namespace Rodonaves.EDI.Model
{
    public class LogInfo : BaseInfo
    {
        public int Id { get; set; }
        public int GenerateReturnId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int Type { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Hour { get; set; }
    }
}
