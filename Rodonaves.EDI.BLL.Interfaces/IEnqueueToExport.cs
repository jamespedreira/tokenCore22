using Rodonaves.EDI.Model;
using Rodonaves.TaskExecutor.Infra;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IEnqueueToExport : IActionObject
    {
        Task<bool> EnqueueAsync(int agreementId);
    }
}
