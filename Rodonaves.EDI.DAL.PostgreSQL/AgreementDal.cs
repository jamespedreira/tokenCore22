using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
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
    public class AgreementDal : BaseCrudDal<AgreementInfo>, IAgreementDal
    {
        private readonly ICustomerDal _customerDal;
        private readonly IPersonDal _personDal;

        #region Ctor

        public AgreementDal(ICustomerDal customerDal, IPersonDal personDal)
        {
            _customerDal = customerDal;
            _personDal = personDal;
        }

        #endregion

        #region SQL

        const string orderBy = @" ORDER BY CCL_IDENTI";

        const string _sqlGetAll = @"SELECT *
                                     FROM EDI_CLICBL
                                     INNER JOIN EDI_CLIENT ON CLI_IDENTI = CCL_CLI_IDENTI
                                     INNER JOIN EDI_PESSOA ON PES_IDENTI = CLI_PES_IDENTI";

        const string _sqlWhereCompany = " WHERE CCL_EMP_IDENTI = :CCL_EMP_IDENTI";

        const string _sqlGetAgreementToExport = _sqlGetAll +
                                                @" WHERE EXISTS (SELECT *
                                                                  FROM EDI_CCLPRC
                                                                 WHERE CLP_CCL_IDENTI = CCL_IDENTI
                                                                  AND (CLP_ULTEXC IS NULL OR
                                                                       CLP_PROEXC   <= :DATE))";

        const string _sqlGetById = _sqlGetAll + 
                                   @" WHERE CCL_IDENTI = :CCL_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<AgreementInfo> fields = new FieldsClass<AgreementInfo>();

        protected override PostgreSQLMapper<AgreementInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(AgreementInfo).Name;

            mapper.TableName = "EDI_CLICBL";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "CCL_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Active), "CCL_ATIVO", false, false, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Raiz), "CCL_RAIZ", false, false, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Customer.Id), "CCL_CLI_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.CompanyId), "CCL_EMP_IDENTI", false, 0, ORMapper.FieldFlags.InsertField, true);
            mapper.OnGetParameterValue += Mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += Mapper_OnSetInstanceValue;

            return mapper;
        }

        void Mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.Active))
                newValue = (bool)value == true ? "S" : "N";
            else if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.Raiz))
                newValue = (bool)value == true ? "S" : "N";
        }
        void Mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.Active))
                newValue = value.ToString() == "S" ? true : false;
            else if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.Raiz))
                newValue = value.ToString() == "S" ? true : false;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_CLICBL";
        }

        #endregion

        #region Methods

        public async Task<List<AgreementInfo>> GetAll(long companyId)
        {
            var list = new List<AgreementInfo>();

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                PostgreSQLHelper.CreateParameter(":CCL_EMP_IDENTI", NpgsqlDbType.Integer, companyId)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll + _sqlWhereCompany, parameters.ToArray()))
                while (rdr.Read())
                {
                    list.Add(NewClassInfoBase(rdr));

                }

            return list;
        }

        public async Task<List<AgreementInfo>> GetByParamsAsync(AgreementInfo info, bool? active, int page, int amountByPage)
        {

            var list = new List<AgreementInfo>();

            var pagination = new OraPaginationInfo
            {
                AmountByPage = amountByPage,
                Page = page,
                FilterParameter = new StringBuilder()
            };

            string sqlCommand = _sqlGetAll,
                   where = " ";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
                {
                    PostgreSQLHelper.CreateParameter(":CCL_EMP_IDENTI", NpgsqlDbType.Integer, info.CompanyId)
                };


            if (info.Id != 0)
            {
                DalUtils.SetFilterOnQuery("CCL_IDENTI", ":ID", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":ID", NpgsqlDbType.Integer, info.Id));
            }

            if (info.Customer.Id != 0)
            {
                DalUtils.SetFilterOnQuery("CLI_IDENTI", ":CLI_IDENTI", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLI_IDENTI", NpgsqlDbType.Integer, info.Customer.Id));
            }

            if (!string.IsNullOrEmpty(info.Customer.Person.Description))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(PES_DESCRI) LIKE :PES_DESCRI", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":PES_DESCRI", NpgsqlDbType.Varchar, "%" + info.Customer.Person.Description.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Customer.Person.TaxIdRegistration))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(PES_CPFCNP) LIKE :PES_CPFCNP", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":PES_CPFCNP", NpgsqlDbType.Varchar, "%" + info.Customer.Person.TaxIdRegistration.ToLower() + "%"));
            }

            if (active != null)
            {
                DalUtils.SetFilterOnQuery("CCL_ATIVO", ":CCL_ATIVO", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CCL_ATIVO", NpgsqlDbType.Varchar, active.Value ? "S" : "N"));
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
        }

        public async override Task<AgreementInfo> GetByIdAsync(int id)
        {
            AgreementInfo entity = null;

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter(":CCL_IDENTI", NpgsqlDbType.Integer, id)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                while (rdr.Read())
                    entity = NewClassInfoBase(rdr);

            return entity;
        }

        public async Task<List<AgreementInfo>> GetAgreementToExport(DateTime referenceDate)
        {
            var list = new List<AgreementInfo>();

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                PostgreSQLHelper.CreateParameter(":DATE", NpgsqlDbType.Date, referenceDate)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAgreementToExport, parameters.ToArray()))
                while (rdr.Read())
                {
                    list.Add(NewClassInfoBase(rdr));
                }

            return list;
        }

        #endregion

        #region Class Info
    
        private AgreementInfo NewClassInfoBase(object reader)
        {
            var info = NewClassInfo(reader);

            info.Customer = _customerDal.NewClassInfo(reader);

            info.Customer.Person = _personDal.NewClassInfo(reader);

            return info;
        }

        public AgreementInfo NewClassInfo(object reader)
        {
            return base.PostgreSQLMapperBaseNew.NewClassInfo<AgreementInfo>(reader as NpgsqlDataReader);
        }

        #endregion
    }
}

