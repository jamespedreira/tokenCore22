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
    public class LayoutHeaderDal : BaseCrudDal<LayoutHeaderInfo>, ILayoutHeaderDal
    {
        #region Properties

        private readonly IScriptDal _scriptDal;

        #endregion

        #region Ctor

        public LayoutHeaderDal(IScriptDal scriptDal)
        {
            _scriptDal = scriptDal;
        }

        #endregion

        #region SQL

        const string orderBy = @" ORDER BY CBL_IDENTI";

        const string _sqlGetAll = @"SELECT *
                                   FROM EDI_CABLAY 
                                   INNER JOIN EDI_SCRLAY ON CBL_SRL_IDENTI = SRL_IDENTI
                                   WHERE CBL_EMP_IDENTI = :CBL_EMP_IDENTI";

        const string _sqlGetById = @"SELECT *
                                   FROM EDI_CABLAY 
                                   INNER JOIN EDI_SCRLAY ON CBL_SRL_IDENTI = SRL_IDENTI 
                                   WHERE CBL_IDENTI = :CBL_IDENTI";

        const string _sqlGetByProcessType = @"SELECT *
                                            FROM EDI_CABLAY
                                            WHERE EXISTS (SELECT 1
			                                               FROM EDI_CCLPRC
			                                              INNER JOIN EDI_CLICBL ON CCL_IDENTI = CLP_CCL_IDENTI
			                                              WHERE CLP_CBL_IDENTI = CBL_IDENTI
			                                               AND CLP_EDIPRC = :CLP_EDIPRC
			                                               AND CCL_EMP_IDENTI = :CCL_EMP_IDENTI
                                                           AND CCL_CLI_IDENTI = :CCL_CLI_IDENTI)";

        #endregion

        #region Mapper

        readonly FieldsClass<LayoutHeaderInfo> fields = new FieldsClass<LayoutHeaderInfo>();

        protected override PostgreSQLMapper<LayoutHeaderInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(LayoutGroupInfo).Name;

            mapper.TableName = "EDI_CABLAY";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "CBL_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.LayoutGroupId), "CBL_GRL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Description), "CBL_DESCRI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Active), "CBL_ATIVO", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ScriptId), "CBL_SRL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.CompanyId), "CBL_EMP_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ProcessType), "CBL_EDIPRC", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.OnGetParameterValue += mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += mapper_OnSetInstanceValue;

            return mapper;
        }

        void mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProcessType))
                newValue = (int)((ProcessTypeEnum)value);
            else if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.Active))
                newValue = (bool)value == true ? "S" : "N";
        }
        void mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProcessType))
                newValue = (ProcessTypeEnum)value;
            else if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.Active))
                newValue = value.ToString() == "S" ? true : false;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_CABLAY";
        }

        #endregion

        #region Methods

        Task<List<LayoutHeaderInfo>> ILayoutHeaderDal.GetAllAsync(long companyId)
        {
            return Task.Run(() =>
            {
                var list = new List<LayoutHeaderInfo>();

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
                parameters.Add(PostgreSQLHelper.CreateParameter(":CBL_EMP_IDENTI", NpgsqlDbType.Integer, companyId));

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll, parameters.ToArray()))
                    while (rdr.Read())
                        list.Add(NewClassInfoBase(rdr));

                return list;
            });
        }

        public Task<List<LayoutHeaderInfo>> GetByParamsAsync(int id, long? layoutGroupId, string description, bool? active, long? scriptId, long companyId, string processType, int page, int amountByPage)
        {
            return Task.Run(() =>
            {
                var list = new List<LayoutHeaderInfo>();

                var pagination = new OraPaginationInfo
                {
                    AmountByPage = amountByPage,
                    Page = page,
                    FilterParameter = new StringBuilder()
                };

                string sqlCommand = _sqlGetAll,
                       where = " ";

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
                parameters.Add(PostgreSQLHelper.CreateParameter(":CBL_EMP_IDENTI", NpgsqlDbType.Integer, companyId));

                if (id != 0)
                {
                    DalUtils.SetFilterOnQuery("CBL_IDENTI", ":ID", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":ID", NpgsqlDbType.Integer, id));
                }

                if (layoutGroupId != null)
                {
                    DalUtils.SetFilterOnQuery("CBL_GRL_IDENTI", ":CBL_GRL_IDENTI", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":CBL_GRL_IDENTI", NpgsqlDbType.Integer, layoutGroupId.Value));
                }

                if (!string.IsNullOrEmpty(description))
                {
                    DalUtils.SetCustomFilterOnQuery("CBL_DESCRI LIKE :CBL_DESCRI", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":CBL_DESCRI", NpgsqlDbType.Varchar, "%" + description + "%"));
                }

                if (active != null)
                {
                    DalUtils.SetFilterOnQuery("CBL_ATIVO", ":CBL_ATIVO", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":CBL_ATIVO", NpgsqlDbType.Varchar, active.Value ? "S" : "N"));
                }

                if (scriptId != null)
                {
                    DalUtils.SetFilterOnQuery("CBL_SRL_IDENTI", ":CBL_SRL_IDENTI", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":CBL_SRL_IDENTI", NpgsqlDbType.Integer, scriptId.Value));
                }

                if (!string.IsNullOrEmpty(processType))
                {
                    DalUtils.SetFilterOnQuery("CBL_EDIPRC", ":CBL_EDIPRC", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":CBL_EDIPRC", NpgsqlDbType.Varchar, processType));
                }

                foreach (NpgsqlParameter item in parameters)
                    pagination.FilterParameter.Append(item.Value);

                sqlCommand += where + orderBy;

                int totalPages = PostgreSQLHelper.GetTotalPages(GetConnectionString(), parameters.ToArray(), sqlCommand, amountByPage);
                using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, sqlCommand, pagination, parameters.ToArray()))
                    while (rdr.Read())
                    {
                        var data = NewClassInfoBase(rdr);
                        data.AmountPages = totalPages;

                        list.Add(data);
                    }


                return list;
            });
        }

        public override Task<LayoutHeaderInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                LayoutHeaderInfo entity = null;

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":CBL_IDENTI", NpgsqlDbType.Integer, id)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = NewClassInfoBase(rdr);

                return entity;
            });
        }

        public Task<List<LayoutHeaderInfo>> GetByProcessType(long companyId, int processType, int customerId, int page, int amountByPage)
        {
            return Task.Run(() =>
            {
                var list = new List<LayoutHeaderInfo>();

                var pagination = new OraPaginationInfo
                {
                    AmountByPage = amountByPage,
                    Page = page,
                    FilterParameter = new StringBuilder()
                };

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
                {
                    PostgreSQLHelper.CreateParameter(":CCL_EMP_IDENTI",NpgsqlDbType.Integer, companyId),
                    PostgreSQLHelper.CreateParameter(":CLP_EDIPRC",NpgsqlDbType.Integer, processType),
                    PostgreSQLHelper.CreateParameter(":CCL_CLI_IDENTI",NpgsqlDbType.Integer, customerId)
                };

                foreach (NpgsqlParameter item in parameters)
                    pagination.FilterParameter.Append(item.Value);

                using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, _sqlGetByProcessType, pagination, parameters.ToArray()))
                    while (rdr.Read())
                        list.Add(NewClassInfoBase(rdr));

                return list;
            });
        }

        #endregion


        private LayoutHeaderInfo NewClassInfoBase(object reader)
        {
            var info = NewClassInfo(reader);

            info.Script = _scriptDal.NewClassInfo(reader);

            return info;
        }

        public LayoutHeaderInfo NewClassInfo(object reader)
        {
            return base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutHeaderInfo>(reader as NpgsqlDataReader);
        }
    }
}
