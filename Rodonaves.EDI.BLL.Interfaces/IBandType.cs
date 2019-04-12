using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IBandType
    {
        Task<List<BandTypeDTO>> GetBandTypes(string processType);
        Task<string> GetBandTypeDescription(string value, ProcessTypeEnum processType);
    }
}
