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
    public class LogDal : BaseCrudDal<LogInfo>, ILogDal
    {
        #region SQL

        private const string _sqlGetById = "SELECT * FROM EDI_LOGGRT WHERE LGR_IDENTI = :LGR_IDENTI";

        private const string _sqlGetByGenerateReturnId = "SELECT * FROM EDI_LOGGRT WHERE LGR_GTN_IDENTI = :LGR_GTN_IDENTI and LGR_TIPO != 4 ORDER BY LGR_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<LogInfo> fields = new FieldsClass<LogInfo>();

        protected override PostgreSQLMapper<LogInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(LogInfo).Name;

            mapper.TableName = "EDI_LOGGRT";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "LGR_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.GenerateReturnId), "LGR_GTN_IDENTI", true, 0, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Title), "LGR_TITULO", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Message), "LGR_MESSAGE", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Type), "LGR_TIPO", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Date), "LGR_DATA", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Hour), "LGR_HORA", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);

            return mapper;
        }

        #endregion

        public override Task<LogInfo> GetByIdAsync(int id)
        {
            return Task.Run(() =>
            {
                LogInfo entity = null;

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":LGR_IDENTI", NpgsqlDbType.Integer, id)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                    while (rdr.Read())
                        entity = NewClassInfo(rdr);

                return entity;
            });
        }

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_LOGGRT";
        }

        public LogInfo NewClassInfo(object reader)
        {
            return base.PostgreSQLMapperBaseNew.NewClassInfo<LogInfo>(reader as NpgsqlDataReader);
        }

        public Task<List<LogInfo>> GetByGenerateReturnIdAsync(long generateReturnId) => Task.Run(() =>
        {
            List<LogInfo> list = new List<LogInfo>();

            var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":LGR_GTN_IDENTI", NpgsqlDbType.Integer, generateReturnId)
                };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetByGenerateReturnId, param.ToArray()))
                while (rdr.Read())
                    list.Add(NewClassInfo(rdr));

            return list;
        });
    }
}
