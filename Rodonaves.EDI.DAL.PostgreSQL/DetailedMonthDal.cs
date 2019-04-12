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
    public class DetailedMonthDal : BaseCrudDal<DetailedMonthInfo>, IDetailedMonthDal
    {

        const string _sqlGetAll = @"SELECT *
                                   FROM EDI_MESDET ";

        const string _sqlGetById = _sqlGetAll +
                                  @" WHERE MDE_CFG_IDENTI = :MDE_CFG_IDENTI";

        const string _delete = @" DELETE FROM EDI_MESDET WHERE MDE_CFG_IDENTI = :MDE_CFG_IDENTI";

        readonly FieldsClass<DetailedMonthInfo> fields = new FieldsClass<DetailedMonthInfo>();
        protected override PostgreSQLMapper<DetailedMonthInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(DetailedMonthInfo).Name;

            mapper.TableName = "EDI_MESDET";

            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ConfigurationId), "MDE_CFG_IDENTI", false, null,
                ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Month), "MDE_MES", false, null,
                ORMapper.FieldFlags.InsertField, false);

            return mapper;
        }

        public Task<List<int?>> GetMonthsByConfigurationIdAsync(int configurationId)
        {
            return Task.Run(() =>
            {
                List<int?> entity = new List<int?>();

                var param = new List<NpgsqlParameter>() { PostgreSQLHelper.CreateParameter(":MDE_CFG_IDENTI", NpgsqlDbType.Integer, configurationId) };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                    {
                        var month = base.PostgreSQLMapperBaseNew.NewClassInfo<DetailedMonthInfo>(rdr)?.Month;
                        if (month != null)
                            entity.Add(month);

                    }

                return entity;
            });
        }

        public override Task<DetailedMonthInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                DetailedMonthInfo entity = null;

                var param = new List<NpgsqlParameter>() { PostgreSQLHelper.CreateParameter(":MDE_CFG_IDENTI", NpgsqlDbType.Integer, id) };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = base.PostgreSQLMapperBaseNew.NewClassInfo<DetailedMonthInfo>(rdr);

                return entity;
            });
        }

        protected override string GetSequenceName()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteByConfigurationId(int configurationId)
        {
            return Task.Run(() =>
            {

                var param = new List<NpgsqlParameter>() { PostgreSQLHelper.CreateParameter(":MDE_CFG_IDENTI", NpgsqlDbType.Integer, configurationId) };

                var rdr = PostgreSQLHelper.ExecuteNonQuery(GetConnectionString(), CommandType.Text, _delete, param.ToArray());
                if (rdr >= 0)
                    return true;
                else
                    return false;
            });
        }
    }
}
