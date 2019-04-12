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
    public class LayoutGroupDal : BaseCrudDal<LayoutGroupInfo>, ILayoutGroupDal
    {
        #region SQL

        const string orderBy = @" ORDER BY GRL_IDENTI";

        const string _sqlGetAll = @"SELECT *
                                   FROM EDI_GRULAY ";

        const string _sqlGetById = _sqlGetAll +
                                   @" WHERE GRL_IDENTI = :GRL_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<LayoutGroupInfo> fields = new FieldsClass<LayoutGroupInfo>();

        protected override PostgreSQLMapper<LayoutGroupInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(LayoutGroupInfo).Name;

            mapper.TableName = "EDI_GRULAY";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "GRL_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Description), "GRL_DESCRI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.DevelopmentCompany), "GRL_EMPDES", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);

            return mapper;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_GRULAY";
        }

        #endregion

        #region Methods

        Task<List<LayoutGroupInfo>> ILayoutGroupDal.GetAll()
        {
            return Task.Run(() =>
            {
                var list = new List<LayoutGroupInfo>();

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll))
                    while (rdr.Read())
                        list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutGroupInfo>(rdr));

                return list;
            });
        }

        public Task<List<LayoutGroupInfo>> GetByParamsAsync(int id, string description, string developmentCompany, int page, int amountByPage)
        {

            return Task.Run(() =>
            {
                var list = new List<LayoutGroupInfo>();

                var pagination = new OraPaginationInfo
                {
                    AmountByPage = amountByPage,
                    Page = page,
                    FilterParameter = new StringBuilder()
                };

                string sqlCommand = _sqlGetAll,
                       where = string.Empty;

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                if (id != 0)
                {
                    DalUtils.SetFilterOnQuery("GRL_IDENTI", ":ID", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":ID", NpgsqlDbType.Integer, id));
                }

                if (!string.IsNullOrEmpty(description))
                {
                    DalUtils.SetCustomFilterOnQuery("GRL_DESCRI LIKE :GRL_DESCRI", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":GRL_DESCRI", NpgsqlDbType.Varchar, "%" + description + "%"));
                }

                if (!string.IsNullOrEmpty(developmentCompany))
                {
                    DalUtils.SetCustomFilterOnQuery("GRL_EMPDES LIKE :GRL_EMPDES", ref where);
                    parameters.Add(PostgreSQLHelper.CreateParameter(":GRL_EMPDES", NpgsqlDbType.Varchar, "%" + developmentCompany + "%"));
                }
                
                foreach (NpgsqlParameter item in parameters)
                    pagination.FilterParameter.Append(item.Value);

                sqlCommand += where + orderBy;

                int totalPages = PostgreSQLHelper.GetTotalPages(GetConnectionString(), parameters.ToArray(), sqlCommand, amountByPage);
                using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, sqlCommand, pagination, parameters.ToArray()))
                    while (rdr.Read())
                    {
                        var provider = base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutGroupInfo>(rdr);
                        provider.AmountPages = totalPages;

                        list.Add(provider);
                    }


                return list;
            });
        }

        public override Task<LayoutGroupInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                LayoutGroupInfo entity = null;

                var param = new List<NpgsqlParameter>() { PostgreSQLHelper.CreateParameter(":GRL_IDENTI", NpgsqlDbType.Integer, id) };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutGroupInfo>(rdr);

                return entity;
            });
        }

        #endregion
    }
}
