using Microsoft.Extensions.Configuration;
using Rodonaves.QueueMessage;

namespace Rodonaves.EDI.Helpers
{
    public class EDIQueueHelper : RTEQueue
    {
        public EDIQueueHelper()
        {
        }

        public EDIQueueHelper(IConfiguration configuration) : base(configuration)
        {
        }

        public EDIQueueHelper(string hostname, string queueName, string username, string password, int? maxPriority, IConfiguration configuration) : base(hostname, queueName, username, password, maxPriority, configuration)
        {
        }
    }
}