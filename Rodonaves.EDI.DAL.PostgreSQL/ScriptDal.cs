using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
using Rodonaves.EDI.Enums;
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
    public class ScriptDal : BaseCrudDal<ScriptInfo>, IScriptDal
    {
        #region SQL

        const string orderBy = @" ORDER BY SRL_IDENTI";

        const string _sqlGetAll = @"SELECT *
                                   FROM EDI_SCRLAY ";

        const string _sqlGetById = _sqlGetAll +
                                   @" WHERE SRL_IDENTI = :SRL_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<ScriptInfo> fields = new FieldsClass<ScriptInfo>();

        protected override PostgreSQLMapper<ScriptInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(ScriptInfo).Name;

            mapper.TableName = "EDI_SCRLAY";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "SRL_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ProviderId), "SRL_PVD_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Description), "SRL_DESCRI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Script), "SRL_SCRIPT", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.BandType), "SRL_TIPBAN", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ProcessType), "SRL_EDIPRC", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.OnGetParameterValue += mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += mapper_OnSetInstanceValue;
            return mapper;
        }

        void mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {

            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProcessType))
                newValue = (int)((ProcessTypeEnum)value);
        }
        void mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProcessType))
                newValue = (ProcessTypeEnum)value;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_SCRLAY";
        }

        #endregion

        #region Get Methods
        
        public override Task<ScriptInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                ScriptInfo entity = null;

                var param = new List<NpgsqlParameter>() { PostgreSQLHelper.CreateParameter(":SRL_IDENTI", NpgsqlDbType.Integer, id) };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = base.PostgreSQLMapperBaseNew.NewClassInfo<ScriptInfo>(rdr);

                return entity;
            });
        }

        public Task<List<ScriptInfo>> GetAll()
        {
            return Task.Run(() =>
            {
                var list = new List<ScriptInfo>();

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll))
                    while (rdr.Read())
                        list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<ScriptInfo>(rdr));

                return list;
            });
        }

        public Task<List<ScriptInfo>> GetByParamsAsync(int? id, int? providerId, string description, string script, string bandType, string processType, int page, int amountByPage)
        {
            return Task.Run(() =>
            {
                var list = new List<ScriptInfo>();

                var pagination = new OraPaginationInfo
                {
                    AmountByPage = amountByPage,
                    Page = page,
                    FilterParameter = new StringBuilder()
                };

                string sqlCommand = _sqlGetAll,
                       where = string.Empty;

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                if (id != null)
                {
                    DalUtils.SetFilterOnQuery("SRL_IDENTI", ":SRL_IDENTI", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":SRL_IDENTI", NpgsqlDbType.Integer, id));
                }

                if (providerId != null)
                {
                    DalUtils.SetFilterOnQuery("SRL_PVD_IDENTI", ":SRL_PVD_IDENTI", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":SRL_PVD_IDENTI", NpgsqlDbType.Integer, providerId));
                }

                if (!string.IsNullOrEmpty(description))
                {
                    DalUtils.SetCustomFilterOnQuery("SRL_DESCRI LIKE :SRL_DESCRI", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":SRL_DESCRI", NpgsqlDbType.Varchar, "%" + description + "%"));
                }

                if (!string.IsNullOrEmpty(script))
                {
                    DalUtils.SetCustomFilterOnQuery("SRL_SCRIPT LIKE :SRL_SCRIPT", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":SRL_SCRIPT", NpgsqlDbType.Varchar, "%" + script + "%"));
                }

                if (!string.IsNullOrEmpty(bandType))
                {
                    DalUtils.SetFilterOnQuery("SRL_TIPBAN", ":SRL_TIPBAN", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":SRL_TIPBAN", NpgsqlDbType.Varchar, bandType));
                }

                if (!string.IsNullOrEmpty(processType))
                {
                    DalUtils.SetFilterOnQuery("SRL_EDIPRC", ":SRL_EDIPRC", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":SRL_EDIPRC", NpgsqlDbType.Varchar, processType));
                }

                foreach (NpgsqlParameter item in parameters)
                    pagination.FilterParameter.Append(item.Value);

                sqlCommand += where + orderBy;

                int totalPages = PostgreSQLHelper.GetTotalPages(GetConnectionString(), parameters.ToArray(), sqlCommand, amountByPage);
                using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, sqlCommand, pagination, parameters.ToArray()))
                    while (rdr.Read())
                    {
                        var scriptInfo = base.PostgreSQLMapperBaseNew.NewClassInfo<ScriptInfo>(rdr);
                        scriptInfo.AmountPages = totalPages;

                        list.Add(scriptInfo);
                    }

                return list;
            });
        }

        public Task<List<ScriptInfo>> GetByProcessTypeAsync(string processType)
        {
            return Task.Run(() =>
            {
                var list = new List<ScriptInfo>();

                var parameters = new List<NpgsqlParameter>();
                string sql = "SELECT SRL_IDENTI, SRL_DESCRI FROM EDI_SCRLAY ";

                if (String.IsNullOrEmpty(processType) == false)
                {
                    sql += "WHERE SRL_EDIPRC = :SRL_EDIPRC";
                    parameters.Add(PostgreSQLHelper.CreateParameter(":SRL_EDIPRC", NpgsqlDbType.Varchar, processType));
                }

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, sql, parameters.ToArray()))
                    while (rdr.Read())
                        list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<ScriptInfo>(rdr));

                return list;
            });
        }

        public Task<List<ScriptInfo>> GetByBandTypeAsync(string processType, string bandType)
        {
            return Task.Run(() =>
            {
                var list = new List<ScriptInfo>();

                var parameters = new List<NpgsqlParameter>();
                string sql = "SELECT SRL_IDENTI, SRL_DESCRI FROM EDI_SCRLAY WHERE SRL_EDIPRC = :SRL_EDIPRC AND SRL_TIPBAN = :SRL_TIPBAN";
                parameters.Add(PostgreSQLHelper.CreateParameter(":SRL_EDIPRC", NpgsqlDbType.Varchar, processType));
                parameters.Add(PostgreSQLHelper.CreateParameter(":SRL_TIPBAN", NpgsqlDbType.Varchar, bandType));

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, sql, parameters.ToArray()))
                    while (rdr.Read())
                        list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<ScriptInfo>(rdr));

                return list;
            });
        }
        
        #endregion

        public ScriptInfo NewClassInfo(object reader)
        {
            return PostgreSQLMapperBaseNew.NewClassInfo<ScriptInfo>(reader as NpgsqlDataReader);
        }
    }
}
