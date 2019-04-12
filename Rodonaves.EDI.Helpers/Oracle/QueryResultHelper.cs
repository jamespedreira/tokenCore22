using Oracle.ManagedDataAccess.Client;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.Engine.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.Oracle
{
    public class QueryResultHelper<T> : QueryResultHelperBase<T, ColumnHeaderHelper>, IQueryResultHelper<T>
    {
        public QueryResultHelper(string connectionString, string query, List<QueryParameter> parameters) : base(connectionString, query, parameters)
        {
            this.columnHeader = new ColumnHeaderHelper(connectionString, this._query, _parameters);
        }

        public async Task<QueryResult<T>> GetQueryResult(int page, int amountByPage)
        {
            var parameters = new List<OracleParameter>();

            foreach (var item in base._parameters)
            {
                parameters.Add(new OracleParameter(item.ParameterName, item.Value));
            }

            try
            {
                _queryResult.AmountPage = OracleHelper.GetTotalPages(_connectionString, parameters.ToArray(), _query, amountByPage);

                _queryResult.Total = await GetTotal(parameters);

                var pagination = GetOraPagination(page, amountByPage, parameters);

                using (var rdr = OracleHelper.ExecuteReaderPagination(_connectionString, CommandType.Text, _query, pagination, parameters.ToArray()))
                    _queryResult = await GetQueryResultBase(rdr);
            }
            catch(Exception ex)
            {
                SetQueryErroMessage(ex);
            }

            return _queryResult;
        }

        public async Task<QueryResult<T>> GetQueryResult()
        {
            var parameters = new List<OracleParameter>();

            foreach (var item in base._parameters)
            {
                parameters.Add(new OracleParameter(item.ParameterName, item.Value));
            }

            try
            {
                _queryResult.Total = await GetTotal(parameters);

                using (var rdr = OracleHelper.ExecuteReader(_connectionString, CommandType.Text, _query, parameters.ToArray()))
                    _queryResult = await GetQueryResultBase(rdr);
            }
            catch (Exception ex)
            {
                SetQueryErroMessage(ex);
            }

            return _queryResult;
        }

        public async Task<int> GetTotal(List<OracleParameter> parameters)
        {
            int total = 0;

            var queryTotal = GetQueryCountTotal(this._query);

            using (var rdr = OracleHelper.ExecuteReader(_connectionString, CommandType.Text, queryTotal, parameters.ToArray()))
            {
                while (rdr.Read())
                {
                    if (rdr["total"] != null)
                        int.TryParse(rdr["total"].ToString(), out total);
                }
            }

            return total;
        }
    }
}
