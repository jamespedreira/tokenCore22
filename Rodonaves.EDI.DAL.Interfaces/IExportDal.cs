using Rodonaves.EDI.Model;
using System.Collections.Generic;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IExportDal
    {
        List<string> GetColumns(string script);
        List<List<ExportInfo>> GetByScript(string script);
    }
}
