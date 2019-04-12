using Npgsql;
using NpgsqlTypes;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class ProviderDal : BaseCrudDal<ProviderInfo>, IProviderDal
    {
        #region SQL

        const string orderBy = @" ORDER BY PVD_IDENTI";

        const string _sqlGetAll = @"SELECT *
                                   FROM EDI_PROVED ";

        const string _sqlGetById = _sqlGetAll +
                                   @" WHERE PVD_IDENTI = :PVD_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<ProviderInfo> fields = new FieldsClass<ProviderInfo>();

        protected override PostgreSQLMapper<ProviderInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(ProviderInfo).Name;

            mapper.TableName = "EDI_PROVED";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "PVD_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ProviderType), "PVD_TIPO", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Name), "PVD_NOME", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Description), "PVD_DESCRI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ConnectionString), "PVD_CONEXA", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.OnGetParameterValue += mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += mapper_OnSetInstanceValue;
            return mapper;
        }
        
        void mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {

            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProviderType))
                newValue = (int)((ProviderTypeEnum)value);
        }
        void mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProviderType))
                newValue = (ProviderTypeEnum)value;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_PROVED";
        }

        #endregion

        #region Methods

        Task<List<ProviderInfo>> IProviderDal.GetAll()
        {
            return Task.Run(() =>
            {
                var list = new List<ProviderInfo>();

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll))
                    while (rdr.Read())
                        list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<ProviderInfo>(rdr));

                return list;
            });
        }

        public Task<List<ProviderInfo>> GetByParamsAsync(int id, int? providerType,string name, string descripton, string connectionString, int page, int amountByPage)
        {

            return Task.Run(() =>
            {
                var list = new List<ProviderInfo>();

                var pagination = new OraPaginationInfo
                {
                    AmountByPage = amountByPage,
                    Page = page,
                    FilterParameter = new StringBuilder()
                };

                string sqlCommand = _sqlGetAll,
                       where = string.Empty;

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                if(id != 0)
                {
                    DalUtils.SetFilterOnQuery("PVD_IDENTI", ":ID", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":ID", NpgsqlDbType.Integer, id));
                }
                
                if (!string.IsNullOrEmpty(name))
                {
                    DalUtils.SetCustomFilterOnQuery("PVD_NOME LIKE :NAME", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":NAME", NpgsqlDbType.Varchar, "%" + name + "%"));
                }

                if (!string.IsNullOrEmpty(descripton))
                {
                    DalUtils.SetCustomFilterOnQuery("PVD_DESCRI LIKE :DESCRIPTION", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":DESCRIPTION", NpgsqlDbType.Varchar, "%" + descripton + "%"));
                }

                if (!string.IsNullOrEmpty(connectionString))
                {
                    DalUtils.SetCustomFilterOnQuery("PVD_CONEXA LIKE :CONNECTIONSTRING", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":CONNECTIONSTRING", NpgsqlDbType.Varchar, "%" + connectionString + "%"));
                }

                if (providerType.HasValue)
                {
                    DalUtils.SetFilterOnQuery("PVD_TIPO", ":PROVIDERTYPE", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":PROVIDERTYPE", NpgsqlDbType.Integer, providerType.GetValueOrDefault()));
                }

                foreach (NpgsqlParameter item in parameters)
                     pagination.FilterParameter.Append(item.Value);

                sqlCommand += where + orderBy;

                int totalPages = PostgreSQLHelper.GetTotalPages(GetConnectionString(), parameters.ToArray(), sqlCommand, amountByPage);
                using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, sqlCommand, pagination, parameters.ToArray()))
                    while (rdr.Read()) {
                        var provider = base.PostgreSQLMapperBaseNew.NewClassInfo<ProviderInfo>(rdr);
                        provider.AmountPages = totalPages;

                        list.Add(provider);
                    }


                return list;
            });
        }

        public override Task<ProviderInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                ProviderInfo entity = null;

                var param = new List<NpgsqlParameter>() { PostgreSQLHelper.CreateParameter(":PVD_IDENTI", NpgsqlDbType.Integer, id) };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = base.PostgreSQLMapperBaseNew.NewClassInfo<ProviderInfo>(rdr);

                return entity;
            });
        }
        #endregion
    }
}

