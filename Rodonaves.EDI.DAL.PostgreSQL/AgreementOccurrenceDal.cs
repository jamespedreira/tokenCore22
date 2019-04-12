using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class AgreementOccurrenceDal : BaseCrudDal<AgreementOccurrenceInfo>, IAgreementOccurrenceDal
    {
        #region SQL

        const string _sqlGetById = @"SELECT *
                                      FROM EDI_CBLNOC
                                     WHERE CLO_IDENTI = :CLO_IDENTI";

        const string _sqlGetByAgreement = @"SELECT * 
                                             FROM EDI_CBLNOC
                                            WHERE CLO_CCL_IDENTI = :CLO_CCL_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<AgreementOccurrenceInfo> fields = new FieldsClass<AgreementOccurrenceInfo>();

        protected override PostgreSQLMapper<AgreementOccurrenceInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(AgreementOccurrenceInfo).Name;

            mapper.TableName = "EDI_CBLNOC";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "CLO_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.AgreementId), "CLO_CCL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.NatureOccurrenceId), "CLO_NOC_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);

            return mapper;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_CBLNOC";
        }
        #endregion

        #region Methods

        public override Task<AgreementOccurrenceInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                AgreementOccurrenceInfo entity = null;

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":CLO_IDENTI", NpgsqlDbType.Integer, id)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = base.PostgreSQLMapperBaseNew.NewClassInfo<AgreementOccurrenceInfo>(rdr);

                return entity;
            });
        }

        public Task<List<AgreementOccurrenceInfo>> GetByAgreementAsync(int agreementId)
        {
            return Task.Run(() =>
            {
                List<AgreementOccurrenceInfo> list = new List<AgreementOccurrenceInfo>();

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":CLO_CCL_IDENTI", NpgsqlDbType.Integer, agreementId)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetByAgreement, param.ToArray()))
                    while (rdr.Read())
                        list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<AgreementOccurrenceInfo>(rdr));
                return list;
            });
        }

    }

    #endregion
}

