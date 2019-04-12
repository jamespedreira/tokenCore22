using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class AgreementProcessDal : BaseCrudDal<AgreementProcessInfo>, IAgreementProcessDal
    {
        #region SQL

        const string _sqlGetById = @"SELECT *
                                      FROM EDI_CCLPRC
                                     WHERE CLP_IDENTI = :CLP_IDENTI";

        const string _sqlGetByAgreement = @"SELECT * 
                                             FROM EDI_CCLPRC
                                            WHERE CLP_CCL_IDENTI = :CLP_CCL_IDENTI";

        const string _sqlGetProcessToExport = _sqlGetByAgreement +
                                                @" AND (CLP_ULTEXC IS NULL OR
                                                        CLP_PROEXC <= :DATA)";

        #endregion

        #region Mapper

        readonly FieldsClass<AgreementProcessInfo> fields = new FieldsClass<AgreementProcessInfo>();

        protected override PostgreSQLMapper<AgreementProcessInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(AgreementProcessInfo).Name;

            mapper.TableName = "EDI_CCLPRC";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "CLP_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.AgreementId), "CLP_CCL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.LayoutHeaderId), "CLP_CBL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.LayoutFileNameId), "CLP_NAR_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ProcessType), "CLP_EDIPRC", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.PeriodicityType), "CLP_PERIOD", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);

            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.LastRun), "CLP_ULTEXC", false, null, ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.NextRun), "CLP_PROEXC", false, null, ORMapper.FieldFlags.UpdateField, false);

            mapper.OnGetParameterValue += Mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += Mapper_OnSetInstanceValue;

            return mapper;
        }

        void Mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProcessType))
                newValue = (int)((ProcessTypeEnum)value);
            else if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.PeriodicityType))
                newValue = (int)((PeriodicityTypeEnum)value);
        }
        void Mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProcessType))
                newValue = (ProcessTypeEnum)value;
            else if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.PeriodicityType))
                newValue = (PeriodicityTypeEnum)value;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_CCLPRC";
        }

        #endregion

        #region Methods

        public override Task<AgreementProcessInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                AgreementProcessInfo entity = null;

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":CLP_IDENTI", NpgsqlDbType.Integer, id)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = base.PostgreSQLMapperBaseNew.NewClassInfo<AgreementProcessInfo>(rdr);

                return entity;
            });
        }

        public async Task<List<AgreementProcessInfo>>GetByAgreementAsync(int agreementId)
        {
            List<AgreementProcessInfo> list = new List<AgreementProcessInfo>();

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter(":CLP_CCL_IDENTI", NpgsqlDbType.Integer, agreementId)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetByAgreement, param.ToArray()))
                while (rdr.Read())
                    list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<AgreementProcessInfo>(rdr));
            return list;
        }

        public async Task<List<AgreementProcessInfo>> GetAgreementProcessToExport(int agreementId, DateTime referenceDate)
        {
            List<AgreementProcessInfo> list = new List<AgreementProcessInfo>();

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter(":CLP_CCL_IDENTI", NpgsqlDbType.Integer, agreementId),
                PostgreSQLHelper.CreateParameter(":DATA", NpgsqlDbType.Date, referenceDate)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetProcessToExport, param.ToArray()))
                while (rdr.Read())
                    list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<AgreementProcessInfo>(rdr));
            return list;
        }
    }

    #endregion
}

