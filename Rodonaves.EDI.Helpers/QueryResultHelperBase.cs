using Newtonsoft.Json;
using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.EDI.Helpers.DTO;

namespace Rodonaves.EDI.Helpers
{
    public abstract class QueryResultHelperBase<T, TColumn> where TColumn : IColumnHeader
    {
        public string _connectionString { get; set; }
        public string _query { get; set; }
        public QueryResult<T> _queryResult { get; set; }
        public List<QueryParameter> _parameters { get; set; }

        public TColumn columnHeader { get; set; }

        protected QueryResultHelperBase(string connectionString, string query, List<QueryParameter> parameters)
        {
            this._connectionString = connectionString;
            this._query = query.Replace(";", "");
            this._queryResult = new QueryResult<T>();
            _parameters = parameters;
        }

        public OraPaginationInfo GetOraPagination<T>(int page, int amountByPage, List<T> parameters) where T : IDataParameter
        {
            var filter = new StringBuilder();

            if(parameters != null)
            {
                foreach (var item in parameters)
                    filter.Append(item.Value);
            }

            return new OraPaginationInfo
            {
                AmountByPage = amountByPage,
                Page = page,
                FilterParameter = filter
            };
        }

        public async Task<QueryResult<T>> GetQueryResultBase(DbDataReader rdr)
        {
            try { 
                if(_queryResult.Columns != null && _queryResult.Columns.Count == 0) { 
                    _queryResult.Columns = columnHeader.GetColumnHeaders(rdr);
                }

                while (rdr.Read()) {
                    _queryResult.Rows.Add(CreateNewObjectByDbDataReader(_queryResult.Columns, rdr));
                }
            }
            catch(Exception ex)
            {
                SetQueryErroMessage(ex);
            }

            return _queryResult;
        }

        private T CreateNewObjectByDbDataReader(List<ColumnHeader> columns, DbDataReader rdr)
        {
            var json =  new StringBuilder("{");
            foreach (var item in columns)
            {
                json.AppendLine(CreateJsonProprerty(item.Name, rdr[item.Name].ToString()));
            }

            json.AppendLine("}");

            return JsonConvert.DeserializeObject<T>(json.ToString());
        }

        private string CreateJsonProprerty(string name, string value)
        {
            return string.Format("'{0}': '{1}',", name.Replace("'", string.Empty), value.Replace("'", string.Empty));
        }

        public void SetQueryErroMessage(Exception ex)
        {
            _queryResult.Success = false;
            _queryResult.Message = ex.Message;
        }

        public string GetQueryCountTotal(string query)
        {
            return string.Format("SELECT COUNT(1) total FROM (\n {0} )", query);
        }
    }
}
