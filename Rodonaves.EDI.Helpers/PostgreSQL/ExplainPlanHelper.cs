using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.EDI.Helpers.PostgreSQL.Enums;
using Rodonaves.Engine.DbHelper;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.PostgreSQL
{
    public class ExplainPlanHelper : IExplainPlan
    {

        private readonly string _connectionString;
        private readonly string _query;
        private readonly ExplainPlanFormattingType _explainFormattingType;
        
        public ExplainPlanHelper(string connectionString, string query, ExplainPlanFormattingType explainFormattingType)
        {
            _connectionString = connectionString;
            _query = query;
            _explainFormattingType = explainFormattingType;
        }

        #region const
        private const string cmdExplain = "EXPLAIN (FORMAT {0}) {1}";
        #endregion

        public Task<string> getExplainPlan()
        {
            return Task.Run(() =>
            {
                var explainPlan = new StringBuilder();
                using (var tran = PostgreSQLHelper.BeginTransaction(_connectionString, IsolationLevel.ReadCommitted))
                {
                    var _cmExplain = String.Format(cmdExplain, _explainFormattingType.ToDescription(), _query);
                    using (var dr = PostgreSQLHelper.ExecuteReader(tran, CommandType.Text, _cmExplain))
                    {
                        while (dr.Read())
                        {
                            explainPlan.AppendLine(dr[0].ToString());
                        }
                    }

                }

                return explainPlan.ToString();
            });
        }

    }
}
