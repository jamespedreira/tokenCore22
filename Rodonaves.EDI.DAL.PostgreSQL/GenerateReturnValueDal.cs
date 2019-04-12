using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class GenerateReturnValueDal : BaseCrudDal<GenerateReturnValueInfo>, IGenerateReturnValueDal
    {
        #region Sql

        const string _sqlGetAll = @"SELECT *
                                     FROM EDI_GTNVLR ";

        const string _sqlGetById = _sqlGetAll + @" WHERE GTV_IDENTI = :GTV_IDENTI";

        const string _sqlGetByGenerateReturnId = _sqlGetAll + @" WHERE GTV_GTN_IDENTI = :GTV_GTN_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<GenerateReturnValueInfo> fields = new FieldsClass<GenerateReturnValueInfo>();

        protected override PostgreSQLMapper<GenerateReturnValueInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(GenerateReturnValueInfo).Name;

            mapper.TableName = "EDI_GTNVLR";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "GTV_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.GenerateReturnId), "GTV_GTN_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Value), "GTV_VALOR", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);

            return mapper;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_GTNVLR";
        }

        #endregion

        #region Class Info

        public GenerateReturnValueInfo NewClassInfo(object reader)
        {
            return base.PostgreSQLMapperBaseNew.NewClassInfo<GenerateReturnValueInfo>(reader as NpgsqlDataReader);
        }

        #endregion

        public async Task<List<GenerateReturnValueInfo>> GetAllAsync()
        {
            var list = new List<GenerateReturnValueInfo>();

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll))
                while (rdr.Read())
                    list.Add(NewClassInfo(rdr));

            return list;
        }

        public override async Task<GenerateReturnValueInfo> GetByIdAsync(int id)
        {
            var info = new GenerateReturnValueInfo();

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter("GTV_IDENTI", NpgsqlDbType.Integer, id)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                while (rdr.Read())
                    info = NewClassInfo(rdr);

            return info;
        }

        public async Task<List<GenerateReturnValueInfo>> GetByGenerateReturnIdAsync(int generateReturnId)
        {
            var list = new List<GenerateReturnValueInfo>();

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter("GTV_GTN_IDENTI", NpgsqlDbType.Integer, generateReturnId)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetByGenerateReturnId, param.ToArray()))
                while (rdr.Read())
                    list.Add(NewClassInfo(rdr));

            return list;
        }

        public async Task<List<GenerateReturnValueInfo>> GetByGenerateReturnIdAsync(int generateReturnId, int page, int amountByPage)
        {
            var list = new List<GenerateReturnValueInfo>();

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter(":GTV_GTN_IDENTI", NpgsqlDbType.Integer, generateReturnId)
            };

            var pagination = new OraPaginationInfo
            {
                AmountByPage = amountByPage,
                Page = page,
                FilterParameter = new StringBuilder()
            };

            foreach (NpgsqlParameter item in param)
                pagination.FilterParameter.Append(item.Value);


            using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, _sqlGetById, pagination, param.ToArray()))
                while (rdr.Read())
                    list.Add(NewClassInfo(rdr));

            return list;
        }
    }
}
