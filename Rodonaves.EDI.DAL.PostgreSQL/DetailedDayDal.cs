using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class DetailedDayDal : BaseCrudDal<DetailedDayInfo>, IDetailedDayDal
    {
        const string _sqlGetAll = @"SELECT *
                                   FROM EDI_DIADET ";

        const string _sqlGetById = _sqlGetAll +
                                  @" WHERE DDE_CFG_IDENTI = :DDE_CFG_IDENTI";

        const string _delete = @" DELETE FROM EDI_DIADET WHERE DDE_CFG_IDENTI = :DDE_CFG_IDENTI";

        readonly FieldsClass<DetailedDayInfo> fields = new FieldsClass<DetailedDayInfo>();
        protected override PostgreSQLMapper<DetailedDayInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(DetailedDayInfo).Name;

            mapper.TableName = "EDI_DIADET";

            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ConfigurationId), "DDE_CFG_IDENTI", false, null,
                ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Day), "DDE_DIA", false, null,
                 ORMapper.FieldFlags.InsertField, false);

            return mapper;
        }


        public Task<List<int>> GetDaysByConfigurationIdAsync(int configurationId)
        {
            return Task.Run(() =>
            {
                List<int> entity = new List<int>();

                var param = new List<NpgsqlParameter>() { PostgreSQLHelper.CreateParameter(":DDE_CFG_IDENTI", NpgsqlDbType.Integer, configurationId) };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<DetailedDayInfo>(rdr).Day);

                return entity;
            });
        }

        public override Task<DetailedDayInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                DetailedDayInfo entity = null;

                var param = new List<NpgsqlParameter>() { PostgreSQLHelper.CreateParameter(":DDE_CFG_IDENTI", NpgsqlDbType.Integer, id) };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = base.PostgreSQLMapperBaseNew.NewClassInfo<DetailedDayInfo>(rdr);

                return entity;
            });
        }

        protected override string GetSequenceName()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByConfigurationId(int configurationId)
        {
            return Task.Run(() =>
            {

                var param = new List<NpgsqlParameter>() { PostgreSQLHelper.CreateParameter(":DDE_CFG_IDENTI", NpgsqlDbType.Integer, configurationId) };

                var rdr = PostgreSQLHelper.ExecuteNonQuery(GetConnectionString(), CommandType.Text, _delete, param.ToArray());
                if (rdr >= 0)
                    return true;
                else
                    return false;
            });
        }
    }
}
