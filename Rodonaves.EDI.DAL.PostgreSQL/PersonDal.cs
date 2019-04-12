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
    public class PersonDal : BaseCrudDal<PersonInfo>, IPersonDal
    {
        #region SQL

        const string orderBy = @" ORDER BY CLI_IDENTI";

        const string _sqlGetAll = @"SELECT P.*
                                   FROM EDI_PESSOA P
                                   WHERE EXISTS(SELECT 1 
                                                 FROM EDI_CLIENT
                                                WHERE CLI_PES_IDENTI = P.PES_IDENTI
                                                  AND CLI_EMP_IDENTI = :CLI_EMP_IDENTI)";

        const string _sqlGetById = @"SELECT *
                                   FROM EDI_CLIENT 
                                   WHERE CLI_IDENTI = :CLI_IDENTI";

        const string _sqlExistPersonId = @"SELECT *
                                           FROM EDI_PESSOA
                                           WHERE PES_CPFCNP = :PES_CPFCNP";

        #endregion

        #region Mapper

        readonly FieldsClass<PersonInfo> fields = new FieldsClass<PersonInfo>();

        protected override PostgreSQLMapper<PersonInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(CustomerInfo).Name;

            mapper.TableName = "EDI_PESSOA";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "PES_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Description), "PES_DESCRI", false, null, ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.TaxIdRegistration), "PES_CPFCNP", false, null, ORMapper.FieldFlags.InsertField, false);

            return mapper;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_PESSOA";
        }

        #endregion

        #region Methods

        public override Task<PersonInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                PersonInfo entity = null;

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":PES_IDENTI", NpgsqlDbType.Integer, id)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = NewClassInfo(rdr);

                return entity;
                });
            }

        Task<List<PersonInfo>> IPersonDal.GetAll(long companyId)
        {
            return Task.Run(() =>
            {
                var list = new List<PersonInfo>();

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
                {
                    PostgreSQLHelper.CreateParameter(":CLI_EMP_IDENTI", NpgsqlDbType.Integer, companyId)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll, parameters.ToArray()))
                    while (rdr.Read())
                        list.Add(NewClassInfo(rdr));

                return list;
            });
        }

        int IPersonDal.ExistPersonId(string taxIdRegistration)
        {
            PersonInfo entity = null;

            var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":PES_CPFCNP", NpgsqlDbType.Varchar, taxIdRegistration)
                };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlExistPersonId, param.ToArray()))
                while (rdr.Read())
                    entity = NewClassInfo(rdr);

            return (entity == null) ? 0 : entity.Id;       
        }

        public PersonInfo NewClassInfo(object reader)
        {
            return base.PostgreSQLMapperBaseNew.NewClassInfo<PersonInfo>(reader as NpgsqlDataReader);
        }

        #endregion
    }
}
