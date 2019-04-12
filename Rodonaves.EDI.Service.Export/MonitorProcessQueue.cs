using System;
using System.Configuration;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.Helpers;
using Rodonaves.Engine;
using Rodonaves.QueueMessage;
using Rodonaves.QueueMessage.Interfaces;

namespace Rodonaves.EDI.Service.Export
{
    public class MonitorProcessQueue : ManageQueue
    {
        private readonly IExport _export;

        public MonitorProcessQueue(IExport export, IRTEQueue queue, IRTEConsumer consumer) : base(queue, consumer)
        {
            _export = export;
        }

        public override async Task<bool> ExecuteProcess(object model, BasicDeliverEventArgs ea)
        {
            bool success = true;

            try
            {
                var hostname = Global.Configuration.GetSection("RTEQueueEDIQueueHelper:hostname").Value;
                var username = Global.Configuration.GetSection("RTEQueueEDIQueueHelper:username").Value;
                var password = Global.Configuration.GetSection("RTEQueueEDIQueueHelper:password").Value;
                int.TryParse(Global.Configuration.GetSection("RTEQueueEDIQueueHelper:maxPriority").Value, out int maxPriority);
                var queueName = GetBodyContent(ea);

                var queue = new EDIQueueHelper(hostname, queueName, username, password, maxPriority, Global.Configuration);

                BasicGetResult result = queue.Channel.BasicGet(queueName, false);

                if(result != null)
                {
                    var success_aux = false;
                    
                    if (int.TryParse(GetBodyContent(result), out int id))
                        success_aux = await _export.GenerateFileByReturnId(id);

                    if (success_aux)
                        queue.MarkMessageRead(result.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }
    }
}
