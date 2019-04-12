using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.EDI.Helpers.Oracle.Enums;
using Rodonaves.Engine.DbHelper;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.Oracle
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
        private const string cmdExplain = "EXPLAIN PLAN FOR {0}";
        private const string selectExplain = "select plan_table_output from table(dbms_xplan.display('plan_table',null,'{0}'))";
        #endregion

        public Task<string> getExplainPlan()
        {
            return Task.Run(() =>
            {

                var explainPlan = new StringBuilder();

                using (var tran = OracleHelper.BeginTransaction(_connectionString, IsolationLevel.ReadCommitted))
                {
                    var _cmdExplain = String.Format(cmdExplain, _query);
                    var _selectExplain = String.Format(selectExplain, _explainFormattingType.ToDescription());

                    OracleHelper.ExecuteNonQuery(tran, CommandType.Text, _cmdExplain);

                    using (var dr = OracleHelper.ExecuteReader(tran, CommandType.Text, _selectExplain))
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
