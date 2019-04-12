using Npgsql;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.Engine.DbHelper;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.PostgreSQL
{
    public class ColumnHeaderHelper : ColumnHeaderBase, IColumnHeader
    {
        public ColumnHeaderHelper(string connectionString, string query, List<QueryParameter> parameters) : base(connectionString, query, parameters)
        {
        }

        public List<ColumnHeader> GetColumnHeaders()
        {
            List<ColumnHeader> list;
            var _parameters = new List<NpgsqlParameter>();

            foreach (var item in Parameters)
            {
                _parameters.Add(new NpgsqlParameter(item.ParameterName, item.Value));
            }

            using (var rdr = PostgreSQLHelper.ExecuteReader(ConnectionString, CommandType.Text, Query, _parameters.ToArray()))
                list = GetColumnHeaders(rdr);

            return list;
        }

        public override List<ColumnHeader> GetColumnHeaders(DbDataReader rdr)
        {
            var list = new List<ColumnHeader>();
            
            foreach (var schema in rdr.GetColumnSchema())
            {
                string columName = schema.BaseColumnName;

                if (!GetPaginationHeaders().Any(c => c.ToUpper() == schema.BaseColumnName.ToUpper())
                    && !list.Exists(x => x.Name == columName))
                    list.Add(new ColumnHeader()
                    {
                        Name = columName,
                        Type = schema.DataTypeName,
                        IsPrimaryKey = schema.IsKey.GetValueOrDefault()
                    });
            }
            return list;
        }

    }
}
