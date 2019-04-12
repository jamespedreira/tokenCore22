using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class LayoutDictionaryDal : BaseCrudDal<LayoutDictionaryInfo>, ILayoutDictionaryDal
    {
        const string _sqlGetById = @"SELECT *
                                     FROM EDI_DICLAY
                                     WHERE DIL_IDENTI = :DIL_IDENTI";

        const string _sqlGetLayoutColumnById = @"SELECT *
                                     FROM EDI_DICLAY
                                     WHERE DIL_DTL_IDENTI = :DIL_DTL_IDENTI";

        readonly FieldsClass<LayoutDictionaryInfo> fields = new FieldsClass<LayoutDictionaryInfo> ();
        protected override PostgreSQLMapper<LayoutDictionaryInfo> CreateMapperNew ()
        {
            var mapper = base.CreateMapperNew ();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof (LayoutDictionaryInfo).Name;
            mapper.TableName = "EDI_DICLAY";
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.Id),
                "DIL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.KeyField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.LayoutColumnId),
                "DIL_DTL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.ReferenceValue),
                "DIL_VALREF", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.TmsValue),
                "DIL_VALTMS", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            return mapper;
        }
        public override Task<LayoutDictionaryInfo> GetByIdAsync (int id) => Task.Run (() =>
        {
            LayoutDictionaryInfo entity = null;

            var param = new List<NpgsqlParameter> ()
            {
                PostgreSQLHelper.CreateParameter (":ACA_IDENTI", NpgsqlDbType.Integer, id)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader (
                GetConnectionString (),
                CommandType.Text,
                _sqlGetById,
                param.ToArray ()))
            {
                while (rdr.Read ())
                    entity = base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutDictionaryInfo> (rdr);

            }

            return entity;
        });

        public Task<List<LayoutDictionaryInfo>> GetByLayoutColumnIdAsync (int layoutColumnId) => Task.Run (() =>
        {
            List<LayoutDictionaryInfo> list = new List<LayoutDictionaryInfo> ();

            var param = new List<NpgsqlParameter> ()
            {
                PostgreSQLHelper.CreateParameter (":DIL_DTL_IDENTI", NpgsqlDbType.Integer, layoutColumnId)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader (
                GetConnectionString (),
                CommandType.Text,
                _sqlGetLayoutColumnById,
                param.ToArray ()))
            {
                while (rdr.Read ())
                    list.Add (base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutDictionaryInfo> (rdr));

            }

            return list;
        });

        protected override string GetSequenceName () => "SEQ_DICLAY";
    }
}