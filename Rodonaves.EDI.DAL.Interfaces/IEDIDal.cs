using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IEDIDal
    {
        Task<List<int>> GetBillOfLadingIdsByInvoiceAsync(string invoice);
    }
}
