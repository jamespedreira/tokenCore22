using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.TaskExecutor.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IMessageBrokerManagement 
    {
        Task<List<QueueDTO>> GetAllQueuesAsync();
    }
}