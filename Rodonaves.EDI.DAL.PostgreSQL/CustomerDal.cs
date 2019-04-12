using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
using Rodonaves.EDI.DTO;
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
    public class CustomerDal : BaseCrudDal<CustomerInfo>, ICustomerDal
    {
        #region Ctor

        private readonly IPersonDal _personDal;

        public CustomerDal(IPersonDal personDal)
        {
            _personDal = personDal;
        }

        #endregion

        #region SQL

        const string orderBy = @" ORDER BY CLI_IDENTI";

        const string _sqlGetAll = @"SELECT *
                                   FROM EDI_CLIENT
                                   INNER JOIN EDI_PESSOA ON CLI_PES_IDENTI = PES_IDENTI
                                   AND CLI_EMP_IDENTI = :CLI_EMP_IDENTI";

        const string _sqlGetById = @"SELECT *
                                   FROM EDI_CLIENT 
                                   INNER JOIN EDI_PESSOA ON CLI_PES_IDENTI = PES_IDENTI
                                   WHERE CLI_IDENTI = :CLI_IDENTI";

        const string _sqlGetBypProcessType = @"SELECT *
                                            FROM EDI_CLIENT
                                            INNER JOIN EDI_PESSOA ON CLI_PES_IDENTI = PES_IDENTI
                                            WHERE EXISTS (SELECT 1
			                                                FROM EDI_CLICBL
			                                                INNER JOIN EDI_CCLPRC ON CCL_IDENTI = CLP_CCL_IDENTI
			                                                WHERE CCL_CLI_IDENTI = CLI_IDENTI
			                                                AND CLP_EDIPRC = :CLP_EDIPRC)
                                            AND CLI_EMP_IDENTI = :CLI_EMP_IDENTI";

        const string _sqlExistCustomer = _sqlGetAll +
                                        @" AND PES_CPFCNP = :PES_CPFCNP";

        #endregion

        #region Mapper

        readonly FieldsClass<CustomerInfo> fields = new FieldsClass<CustomerInfo>();

        protected override PostgreSQLMapper<CustomerInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(CustomerInfo).Name;

            mapper.TableName = "EDI_CLIENT";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "CLI_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.IdPerson), "CLI_PES_IDENTI", false, null, ORMapper.FieldFlags.FKField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.CompanyId), "CLI_EMP_IDENTI", false, 0, ORMapper.FieldFlags.InsertField, false);

            return mapper;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_CLIENT";
        }

        #endregion

        #region Methods

        public override Task<CustomerInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                CustomerInfo entity = null;

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":CLI_IDENTI", NpgsqlDbType.Integer, id)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = NewClassInfoBase(rdr);

                return entity;
            });
        }

        int ICustomerDal.ExistCustomerId(string taxIdRegistration, long companyId)
        {
            CustomerInfo entity = null;

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                PostgreSQLHelper.CreateParameter(":CLI_EMP_IDENTI", NpgsqlDbType.Integer, companyId),
                PostgreSQLHelper.CreateParameter(":PES_CPFCNP", NpgsqlDbType.Varchar, taxIdRegistration)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlExistCustomer, parameters.ToArray()))
                while (rdr.Read())
                    entity = NewClassInfoBase(rdr);

            return (entity == null) ? 0 : entity.Id;
        }

        Task<List<CustomerInfo>> ICustomerDal.GetAll(long companyId)
        {
            return Task.Run(() =>
            {
                var list = new List<CustomerInfo>();

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
                {
                    PostgreSQLHelper.CreateParameter(":CLI_EMP_IDENTI", NpgsqlDbType.Integer, companyId)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll, parameters.ToArray()))
                    while (rdr.Read())
                        list.Add(NewClassInfoBase(rdr));

                return list;
            });
        }

        public Task<List<CustomerInfo>> GetByParams(CustomerInfo info, int page, int amountByPage)
        {
            return Task.Run(() =>
            {
                var list = new List<CustomerInfo>();

                var pagination = new OraPaginationInfo
                {
                    AmountByPage = amountByPage,
                    Page = page,
                    FilterParameter = new StringBuilder()
                };

                string sqlCommand = _sqlGetAll;

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
                {
                    PostgreSQLHelper.CreateParameter(":CLI_EMP_IDENTI",NpgsqlDbType.Integer, info.CompanyId)
                };

                if(info.Person != null)
                {
                    if (!string.IsNullOrEmpty(info.Person.Description))
                    {
                        DalUtils.SetCustomFilterOnQuery("LOWER(PES_DESCRI) LIKE :PES_DESCRI", ref sqlCommand);
                        parameters.Add(PostgreSQLHelper.CreateParameter(":PES_DESCRI", NpgsqlDbType.Varchar, "%" + info.Person.Description.ToLower() + "%"));
                    }

                    if (!string.IsNullOrEmpty(info.Person.TaxIdRegistration))
                    {
                        DalUtils.SetCustomFilterOnQuery("LOWER(PES_CPFCNP) LIKE :PES_CPFCNP", ref sqlCommand);
                        parameters.Add(PostgreSQLHelper.CreateParameter(":PES_CPFCNP", NpgsqlDbType.Varchar, "%" + info.Person.TaxIdRegistration.ToLower() + "%"));
                    }
                }

                foreach (NpgsqlParameter item in parameters)
                    pagination.FilterParameter.Append(item.Value);

                using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, sqlCommand, pagination, parameters.ToArray()))
                    while (rdr.Read())
                        list.Add(NewClassInfoBase(rdr));

                return list;
            });
        }

        #endregion

        public CustomerInfo NewClassInfo(object reader)
        {
            return PostgreSQLMapperBaseNew.NewClassInfo<CustomerInfo>(reader as NpgsqlDataReader);
        }

        private CustomerInfo NewClassInfoBase(object reader)
        {
            var info = PostgreSQLMapperBaseNew.NewClassInfo<CustomerInfo>(reader as NpgsqlDataReader);

            info.Person = _personDal.NewClassInfo(reader);

            return info;
        }

        public Task<List<CustomerInfo>> GetByProcessType(long companyId, int processType, int page, int amountByPage)
        {
            return Task.Run(() =>
            {
                var list = new List<CustomerInfo>();

                var pagination = new OraPaginationInfo
                {
                    AmountByPage = amountByPage,
                    Page = page,
                    FilterParameter = new StringBuilder()
                };

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
                {
                    PostgreSQLHelper.CreateParameter(":CLI_EMP_IDENTI",NpgsqlDbType.Integer, companyId),
                    PostgreSQLHelper.CreateParameter(":CLP_EDIPRC",NpgsqlDbType.Integer, processType)
                };

                foreach (NpgsqlParameter item in parameters)
                    pagination.FilterParameter.Append(item.Value);

                using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, _sqlGetBypProcessType, pagination, parameters.ToArray()))
                    while (rdr.Read())
                        list.Add(NewClassInfoBase(rdr));

                return list;
            });
        }
    }
}
