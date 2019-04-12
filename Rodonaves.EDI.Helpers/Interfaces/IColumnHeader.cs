using Rodonaves.EDI.Helpers.DTO;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.Interfaces
{
    public interface IColumnHeader
    {
        List<ColumnHeader> GetColumnHeaders();
        List<ColumnHeader> GetColumnHeaders(DbDataReader rdr);
    }
}
