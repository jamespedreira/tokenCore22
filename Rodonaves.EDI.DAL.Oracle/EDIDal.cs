using Oracle.ManagedDataAccess.Client;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Model;
using Rodonaves.Engine;
using Rodonaves.Engine.DbHelper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Oracle
{
    public class EDIDal : OracleDal<OperationDetailedInfo>, IEDIDal
    {
        #region SQL

        const string _sqlGetBillOfLadingIdsByInvoice = @"SELECT CTRC FROM EDI_OCOREN50_542 WHERE NUMERO = :NUMERO AND SERIE = :SERIE";

        #endregion

        #region Methods

        public Task<List<int>> GetBillOfLadingIdsByInvoiceAsync(string invoice)
        {
            return Task.Run(() =>
            {
                var list = new List<int>();
                var splitedInvoice = invoice.Split('-');
                if (splitedInvoice.Length < 2)
                    return list;

                var param = new List<OracleParameter>()
                {
                    OracleHelper.CreateParameter(":NUMERO", OracleDbType.Varchar2, splitedInvoice[0]),
                    OracleHelper.CreateParameter(":SERIE", OracleDbType.Varchar2, splitedInvoice[1])
                };

                using (var rdr = OracleHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetBillOfLadingIdsByInvoice, param.ToArray()))
                    while (rdr.Read())
                        list.Add(rdr.GetInt32(0));

                return list;
            });
        }
        
        protected override string GetConnectionString()
        {
            return Global.GetConnectionString("EDI");
        }

        protected override string GetSequenceName()
        {
            return null;
        }

        #endregion
    }
}
