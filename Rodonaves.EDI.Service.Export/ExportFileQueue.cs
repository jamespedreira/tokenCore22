using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.QueueMessage.Interfaces;

namespace Rodonaves.EDI.Service.Export
{
    public class ExportFileQueue : ManageQueue
    {
        private readonly IExport _export;

        public ExportFileQueue(IExport export, IRTEQueue queue, IRTEConsumer consumer) : base(queue, consumer)
        {
            _export = export;
        }

        public override async Task<bool> ExecuteProcess(object model, BasicDeliverEventArgs ea)
        {
            var success = false;

            if (int.TryParse(GetBodyContent(ea), out int id))
                success = await _export.GenerateFileByReturnId(id);

            return success;
        }
    }
}
