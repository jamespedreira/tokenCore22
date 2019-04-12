using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class LayoutFileNameDal : BaseCrudDal<LayoutFileNameInfo>, ILayoutFileNameDal
    {
        const string _sqlGetAll = @"SELECT *
                                     FROM EDI_NOMARQ ";

        const string _sqlGetById = _sqlGetAll +
                                    " WHERE NAR_IDENTI = :NAR_IDENTI";


        const string _sqlGetByProcessType = @"SELECT *
                                                FROM EDI_NOMARQ
                                                WHERE EXISTS (SELECT 1
			                                                    FROM EDI_CCLPRC
			                                                    INNER JOIN EDI_CLICBL ON CCL_IDENTI = CLP_CCL_IDENTI
			                                                    WHERE CLP_NAR_IDENTI = NAR_IDENTI
			                                                    AND CLP_EDIPRC = :CLP_EDIPRC
			                                                    AND CCL_EMP_IDENTI = :CCL_EMP_IDENTI
                                                                AND CCL_CLI_IDENTI = :CCL_CLI_IDENTI)";

        const string _sqlGetByLayoutHeaderId = @"SELECT * 
                                                 FROM EDI_NOMARQ
                                                 WHERE NAR_CBL_IDENTI = :NAR_CBL_IDENTI";

        readonly FieldsClass<LayoutFileNameInfo> fields = new FieldsClass<LayoutFileNameInfo>();

        protected override string GetSequenceName() => "SEQ_EDI_NOMARQ";

        protected override PostgreSQLMapper<LayoutFileNameInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(LayoutFileNameInfo).Name;
            mapper.TableName = "EDI_NOMARQ";
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Id),
                "NAR_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.KeyField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.LayoutHeaderId),
                "NAR_CBL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Description),
                "NAR_DESCRI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Active),
                "NAR_ATIVO", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Default),
                "NAR_PADRAO", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);

            mapper.OnSetInstanceValue += new OnSetInstanceValueEvent(mapper_OnSetInstanceValue);
            mapper.OnGetParameterValue += new OnGetParameterValueEvent(mapper_OnGetParameterValue);
            return mapper;
        }
        void mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == classFields.GetFieldNameFull(p => p.Default))
                newValue = (bool)value ? "S" : "N";

            if (orLineMapper.PropertyName == classFields.GetFieldNameFull(p => p.Active))
                newValue = (bool)value ? "S" : "N";
        }
        void mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == classFields.GetFieldNameFull(p => p.Default))
                newValue = value.ToString() == "S";

            if (orLineMapper.PropertyName == classFields.GetFieldNameFull(p => p.Active))
                newValue = value.ToString() == "S";
        }

        public override Task<LayoutFileNameInfo> GetByIdAsync(int id) => Task.Run(() =>
        {
            LayoutFileNameInfo entity = null;

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter (":NAR_IDENTI", NpgsqlDbType.Integer, id)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(
                GetConnectionString(),
                CommandType.Text,
                _sqlGetById,
                param.ToArray()))
            {
                while (rdr.Read())
                    entity = base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutFileNameInfo>(rdr);

            }

            return entity;
        });

        public Task<List<LayoutFileNameInfo>> GetByLayoutHeaderIdAsync(long layoutHeaderId) => Task.Run(() =>
        {
            List<LayoutFileNameInfo> list = new List<LayoutFileNameInfo>();

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter (":NAR_CBL_IDENTI", NpgsqlDbType.Integer, layoutHeaderId)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(
                GetConnectionString(),
                CommandType.Text,
                _sqlGetByLayoutHeaderId,
                param.ToArray()))
            {
                while (rdr.Read())
                    list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutFileNameInfo>(rdr));

            }

            return list;
        });

        public Task<List<LayoutFileNameInfo>> GetByLParamsAsync(LayoutFileNameInfo info, int page, int amountByPage)
        {
            return Task.Run(() =>
            {
                var list = new List<LayoutFileNameInfo>();

                var pagination = new OraPaginationInfo
                {
                    AmountByPage = amountByPage,
                    Page = page,
                    FilterParameter = new StringBuilder()
                };

                string sqlCommand = _sqlGetAll,
                       where = " ";

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                if (info.Id != 0)
                {
                    DalUtils.SetFilterOnQuery("NAR_IDENTI", ":NAR_IDENTI", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":NAR_IDENTI", NpgsqlDbType.Integer, info.Id));
                }

                if (info.LayoutHeaderId != 0)
                {
                    DalUtils.SetFilterOnQuery("NAR_CBL_IDENTI", ":NAR_CBL_IDENTI", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":NAR_CBL_IDENTI", NpgsqlDbType.Integer, info.LayoutHeaderId));
                }

                if (!string.IsNullOrEmpty(info.Description))
                {
                    DalUtils.SetCustomFilterOnQuery("LOWER(NAR_DESCRI) LIKE :NAR_DESCRI", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":NAR_DESCRI", NpgsqlDbType.Varchar, "%" + info.Description.ToLower() + "%"));
                }

                foreach (NpgsqlParameter item in parameters)
                    pagination.FilterParameter.Append(item.Value);

                sqlCommand += where;

                int totalPages = PostgreSQLHelper.GetTotalPages(GetConnectionString(), parameters.ToArray(), sqlCommand, amountByPage);
                using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, sqlCommand, pagination, parameters.ToArray()))
                    while (rdr.Read())
                    {
                        var data = PostgreSQLMapperBaseNew.NewClassInfo<LayoutFileNameInfo>(rdr);
                        data.AmountPages = totalPages;

                        list.Add(data);
                    }


                return list;
            });
        }

        public Task<List<LayoutFileNameInfo>> GetAllAsync()
        {
            return Task.Run(() =>
            {
                var list = new List<LayoutFileNameInfo>();

                string sqlCommand = _sqlGetAll;

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, sqlCommand, null))
                    while (rdr.Read())
                        list.Add(PostgreSQLMapperBaseNew.NewClassInfo<LayoutFileNameInfo>(rdr));

                return list;
            });
        }

        public Task<List<LayoutFileNameInfo>> GetByProcessType(long companyId, int processType, int customerId, int page, int amountByPage)
        {
            return Task.Run(() =>
            {
                var list = new List<LayoutFileNameInfo>();

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
                    PostgreSQLHelper.CreateParameter(":CCL_CLI_IDENTI",NpgsqlDbType.Integer, customerId),
                };

                foreach (NpgsqlParameter item in parameters)
                    pagination.FilterParameter.Append(item.Value);

                using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, _sqlGetByProcessType, pagination, parameters.ToArray()))
                    while (rdr.Read())
                        list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutFileNameInfo>(rdr));

                return list;
            });
        }
    }
}
