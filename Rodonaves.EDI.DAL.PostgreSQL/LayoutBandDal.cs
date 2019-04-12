using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class LayoutBandDal : BaseCrudDal<LayoutBandInfo>, ILayoutBandDal
    {
        #region Properties

        private readonly IScriptDal _scriptDal;

        #endregion

        #region Ctor

        public LayoutBandDal(IScriptDal scriptDal)
        {
            _scriptDal = scriptDal;
        }
        #endregion

        #region SQL

        const string orderBy = @" ORDER BY BNL_SEQUEN";

        const string _sqlGetAll = @"SELECT *
                                    FROM EDI_BANLAY
                                    INNER JOIN EDI_SCRLAY ON BNL_SRL_IDENTI = SRL_IDENTI";

        const string _sqlGetByLayoutHeader = _sqlGetAll +
                                            @" WHERE BNL_CBL_IDENTI = :BNL_CBL_IDENTI
                                               ORDER BY BNL_SEQUEN";

        const string _sqlGetById = _sqlGetAll + 
                                  @" WHERE BNL_IDENTI = :BNL_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<LayoutBandInfo> fields = new FieldsClass<LayoutBandInfo>();

        protected override PostgreSQLMapper<LayoutBandInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(LayoutGroupInfo).Name;

            mapper.TableName = "EDI_BANLAY";
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Id), "BNL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.KeyField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.LayoutHeaderId), "BNL_CBL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Break), "BNL_QUEBRA", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Sequence), "BNL_SEQUEN", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.ScriptId), "BNL_SRL_IDENTI", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.BandTypeId), "BNL_TIPBAN", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.KeyBand), "BNL_REGIST", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.KeyParentBand), "BNL_REGPAI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.OnGetParameterValue += mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += mapper_OnSetInstanceValue;

            return mapper;
        }

        void mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == classFields.GetFieldNameFull(p => p.Break))
                newValue = (bool)value ? "S" : "N";
        }
        void mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == classFields.GetFieldNameFull(p => p.Break))
                newValue = value.ToString() == "S";
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_BANLAY";
        }

        #endregion

        public override Task<LayoutBandInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                LayoutBandInfo entity = null;

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":BNL_IDENTI", NpgsqlDbType.Integer, id)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = NewClassInfoBase(rdr);

                return entity;
            });
        }

        public Task<List<LayoutBandInfo>> GetByLayoutHeaderAsync(long layoutHeaderId)
        {
            return Task.Run(() =>
            {
                List<LayoutBandInfo> list = new List<LayoutBandInfo>();

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":BNL_CBL_IDENTI", NpgsqlDbType.Integer, layoutHeaderId)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetByLayoutHeader, param.ToArray()))
                    while (rdr.Read())
                        list.Add(NewClassInfoBase(rdr));

                return list;
            });
        }


        private LayoutBandInfo NewClassInfoBase(object reader)
        {
            var info = NewClassInfo(reader);

            info.Script = _scriptDal.NewClassInfo(reader);

            return info;
        }

        public LayoutBandInfo NewClassInfo(object reader)
        {
            return base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutBandInfo>(reader as NpgsqlDataReader);
        }
    }
}
