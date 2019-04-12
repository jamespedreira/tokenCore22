using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Service.Export.Interfaces
{
    public interface IManageQueue : IDisposable
    {
        Task Execute();
        Task<bool> ExecuteProcess(object model, BasicDeliverEventArgs ea);
    }
}
