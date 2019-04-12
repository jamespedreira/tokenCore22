using Rodonaves.EDI.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface INatureOccurrence 
    {
        Task<List<NatureOccurrenceDTO>> GetNatureOccurrences();
    }
}
