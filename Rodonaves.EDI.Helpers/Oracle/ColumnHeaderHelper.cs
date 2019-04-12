using Oracle.ManagedDataAccess.Client;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.Engine.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.Oracle
{
    public class ColumnHeaderHelper : ColumnHeaderBase, IColumnHeader
    {
        public ColumnHeaderHelper(string connectionString, string query, List<QueryParameter> parameters) : base(connectionString, query, parameters)
        {
        }

        public List<ColumnHeader> GetColumnHeaders()
        {
            List<ColumnHeader> list;

            var _parameters = new List<OracleParameter>();

            foreach (var item in Parameters)
            {
                _parameters.Add(new OracleParameter(item.ParameterName, item.Value));
            }

            try
            {
                using (var rdr = OracleHelper.ExecuteReader(ConnectionString, CommandType.Text, Query, _parameters.ToArray()))
                    list = GetColumnHeaders(rdr);
            }
            catch (Exception ex)
            {
                throw;
            }
           

            return list;
        }

        public override List<ColumnHeader> GetColumnHeaders(DbDataReader rdr)
        {
            var list = new List<ColumnHeader>();

            var schema = rdr.GetSchemaTable();

            for (int i = 0; i < schema.Rows.Count; i++)
            {
                bool.TryParse(schema.Rows[i]["ISKEY"].ToString(), out bool iskey);
                string columName = schema.Rows[i]["COLUMNNAME"].ToString();
                if (!GetPaginationHeaders().Any(c => c.ToUpper() == schema.Rows[i]["COLUMNNAME"].ToString().ToUpper()) 
                    && !list.Exists(x => x.Name == columName))
                    list.Add(new ColumnHeader()
                    {
                        Name = columName,
                        Type = schema.Rows[i]["DATATYPE"].ToString(),
                        IsPrimaryKey = iskey
                    });
            }
            return list;
        }
    }
}
