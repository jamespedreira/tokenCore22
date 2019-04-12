using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.TaskExecutor.Infra;
using System;

namespace Rodonaves.EDI.BLL
{
    public class DatabaseLogger : ILogger
    {
        private readonly ILog _log;

        public DatabaseLogger(ILog log)
        {
            _log = log;
        }

        public System.Threading.Tasks.Task LogAsync(string externalId, string title, string message, LogType type)
        {
            var now = DateTime.Now;

            return _log.InsertAsync(new DTO.LogDTO
            {
                GenerateReturnId = String.IsNullOrEmpty(externalId) == false ? Convert.ToInt32(externalId) : 0,
                Message = message,
                Title = title,
                Type = (int)type,
                Date = DateTime.Now,
                Hour = new TimeSpan(now.Hour, now.Minute, now.Second)
            });
        }
    }
}
