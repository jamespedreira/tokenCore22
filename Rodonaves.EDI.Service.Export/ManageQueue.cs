using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rodonaves.EDI.Service.Export.Interfaces;
using Rodonaves.QueueMessage.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Service.Export
{
    public abstract class ManageQueue : IManageQueue
    {
        private readonly IRTEQueue _queue;
        private readonly IRTEConsumer _consumer;

        public ManageQueue(IRTEQueue queue, IRTEConsumer consumer)
        {
            _queue = queue;
            _consumer = consumer;

        }

        public void Dispose()
        {
            _queue.Dispose();
        }

        public async Task Execute()
        {
            _consumer.Received += async (model, ea) =>
            {
                bool success = false;
                try
                {
                    success = await ExecuteProcess(model, ea);
                }
                catch (Exception ex)
                {
                    success = false;
                }

                if (success)
                    _queue.MarkMessageRead(ea.DeliveryTag);
                else
                    _queue.RejectMessage(ea.DeliveryTag);
            };

            _queue.GetChannel().BasicQos(0, 10, false);
            _queue.AddConsumer(_consumer);
        }

        protected string GetBodyContent(BasicDeliverEventArgs ea)
        {
            return Encoding.UTF8.GetString(ea.Body);
        }

        protected string GetBodyContent(BasicGetResult result)
        {
            return Encoding.UTF8.GetString(result.Body);
        }

        public abstract Task<bool> ExecuteProcess(object model, BasicDeliverEventArgs ea);
    }
}
