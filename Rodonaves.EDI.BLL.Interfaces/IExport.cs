using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.TaskExecutor.Infra;
using System;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IExport : IActionObject
    {
        Task<bool> GenerateFileByReturnId(int generateReturnId);
    }
}
