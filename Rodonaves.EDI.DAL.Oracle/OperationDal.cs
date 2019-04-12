using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using Rodonaves.Engine;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;

namespace Rodonaves.EDI.DAL.Oracle
{
    public class OperationDal : OracleDal<OperationDetailedInfo>, IOperationDal
    {
        #region SQL
        const string _sqlGetTotalByStatus = @"SELECT 
                                                COUNT(1) TOTAL,
                                                CTR.CTR_STAENT
                                            FROM 
                                                TFR_CTRC CTR 
                                            GROUP BY 
                                                CTR.CTR_STAENT";
        #endregion

        #region Mapper
        readonly FieldsClass<OperationDetailedInfo> fields = new FieldsClass<OperationDetailedInfo> ();

        protected override OracleMapper<OperationDetailedInfo> CreateMapperNew ()
        {
            OracleMapper<OperationDetailedInfo> mapper = base.CreateMapperNew ();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof (ActionInfo).Name;

            mapper.TableName = "tfr_ctrc";
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.TotalItems), "TOTAL", false, null,
                ORMapper.FieldFlags.NonBaseTableField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.ProgressStatus), "CTR_STAENT", false, null,
                ORMapper.FieldFlags.NonBaseTableField, false);

          return mapper;
        }
        void Mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProgressStatus))
                newValue = (int)((DeliveryProgressEnum)value);
        }
        void Mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProgressStatus))
                newValue = (DeliveryProgressEnum)(int)value;
        }

        #endregion

        #region Methods
        public Task<List<OperationDetailedInfo>> GetTotalByStatusAsync () => Task.Run (() =>
        {
            var list = new List<OperationDetailedInfo> ();
            using (var rdr = OracleHelper.ExecuteReader (GetConnectionString (), CommandType.Text, _sqlGetTotalByStatus))
            while (rdr.Read ())
            {
                list.Add (base.OracleMapperBaseNew.NewClassInfo<OperationDetailedInfo> (rdr));
            }

            return list;
        });

        protected override string GetSequenceName ()
        {
            return "";
        }

        protected override string GetConnectionString ()
        {
            return Global.GetConnectionString ("TMS");
        }
        #endregion
    }
}