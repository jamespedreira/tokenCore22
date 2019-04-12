using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class ExportDal : IExportDal
    {
        public List<List<ExportInfo>> GetByScript(string script)
        {
            throw new NotImplementedException();
        }

        public List<string> GetColumns(string script)
        {
            throw new NotImplementedException();
        }
    }
}
