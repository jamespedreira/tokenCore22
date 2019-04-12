using System.Collections.Generic;

namespace Rodonaves.EDI.Helpers.DTO
{
    public class QueryResult<T>
    {
        public QueryResult()
        {
            this.Columns = new List<ColumnHeader>();
            this.Rows = new List<T>();
        }

        public List<ColumnHeader> Columns { get; set; }

        public int AmountPage { get; set; }

        public int Total { get; set; }

        public List<T> Rows { get; set; }

        public bool Success { get; set; } = true;

        public string Message { get; set; } = string.Empty;
    }
}
