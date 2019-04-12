using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.DTO;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers
{
    public abstract class ColumnHeaderBase
    {
        public string ConnectionString { get; set; }
        public string Query { get; set; }
        public List<QueryParameter> Parameters { get; set; }
        
        public ColumnHeaderBase(string connectionString, string query, List<QueryParameter> parameters)
        {
            this.ConnectionString = connectionString;
            this.Query = query;
            this.Parameters = parameters;

            this.RemoveParametersFromQuery();
        }

        public abstract List<ColumnHeader> GetColumnHeaders(DbDataReader rdr);

        protected IEnumerable<string> GetPaginationHeaders()
        {
            yield return "PAGINA";
            yield return "RNUM";
            yield return "RNUMMIN";
        }

        protected void RemoveParametersFromQuery()
        {
            var parameters = new Regex(@":\w+").Matches(Query);

            foreach (var item in parameters)
                Query = Query.Replace(item.ToString(), "NULL");
        }
    }
}
