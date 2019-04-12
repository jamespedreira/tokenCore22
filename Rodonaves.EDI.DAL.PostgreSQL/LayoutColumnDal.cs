using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.DbHelper;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class LayoutColumnDal : BaseCrudDal<LayoutColumnInfo>, ILayoutColumnDal
    {
        #region SQL
        const string _sqlGetByLayoutBand = @"SELECT *
                                               FROM EDI_DETLAY 
                                               WHERE DTL_BNL_IDENTI = :DTL_BNL_IDENTI
                                               ORDER BY DTL_SEQUEN";


        const string _sqlGetByLayoutFileName = @"SELECT *
                                                FROM EDI_DETLAY 
                                                WHERE DTL_NAR_IDENTI = :DTL_NAR_IDENTI
                                                ORDER BY DTL_SEQUEN";

        const string _sqlGetById = @"SELECT *
                                     FROM EDI_DETLAY 
                                     WHERE DTL_IDENTI = :DTL_IDENTI";

        #endregion

        #region Private Properties
        private string className = typeof (LayoutColumnInfo).Name;
        #endregion

        protected override PostgreSQLMapper<LayoutColumnInfo> CreateMapperNew ()
        {
            var mapper = base.CreateMapperNew ();

            if (!base.DoMapperBase)
                return mapper;

            mapper.TableName = "EDI_DETLAY";
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Id), "DTL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.KeyField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Sequence), "DTL_SEQUEN", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Begin), "DTL_COLINI", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.End), "DTL_COLFIM", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Visual), "DTL_VISUAL", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Comments), "DTL_COMENT", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.ColumnType), "DTL_TIPEXP", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.LayoutFileNameId), "DTL_NAR_IDENTI", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.LayoutBandId), "DTL_BNL_IDENTI", true, 0, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            //mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.ColumnValue.Property), "DTL_VALEXP", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.ColumnFormatType), "DTL_TIPFOR", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.ControlBreak), "DTL_CTRQUE", true, false, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Register), "DTL_REGIST", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.CustomFreightDataId), "DTL_IAD_IDENTI", true, 0, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Element), "DTL_DIRELE", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.ElementValue), "DTL_ELEMEN", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, classFields.GetFieldNameFull(p => p.Multiple), "DTL_MULTIP", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.OnSetInstanceValue += new OnSetInstanceValueEvent(mapper_OnSetInstanceValue);
            mapper.OnGetParameterValue += new OnGetParameterValueEvent(mapper_OnGetParameterValue);
            return mapper;
        }

        void mapper_OnGetParameterValue (ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == classFields.GetFieldNameFull (p => p.ControlBreak))
                newValue = (bool) value ? "S" : "N";
            else if (orLineMapper.PropertyName == classFields.GetFieldNameFull (p => p.ColumnType))
                newValue = EDIEnumsList.GetColumnType ().Find (p => (ColumnType) p.Enum == (ColumnType) value).Value;
            else if (orLineMapper.PropertyName == classFields.GetFieldNameFull (p => p.Multiple))
                newValue = (bool) value ? "S" : "N";
        }
        void mapper_OnSetInstanceValue (ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == classFields.GetFieldNameFull (p => p.ControlBreak))
                newValue = value.ToString () == "S";
            else if (orLineMapper.PropertyName == classFields.GetFieldNameFull (p => p.ColumnType))
                newValue = EDIEnumsList.GetColumnType ().Find (p => p.Value == value.ToString ()).Enum;
            else if (orLineMapper.PropertyName == classFields.GetFieldNameFull (p => p.Multiple))
                newValue = value.ToString () == "S";
        }

        public override Task<LayoutColumnInfo> GetByIdAsync (int id)
        {
            return Task.Run (() =>
            {
                LayoutColumnInfo entity = null;

                var param = new List<NpgsqlParameter> ()
                {
                    PostgreSQLHelper.CreateParameter (":DTL_IDENTI", NpgsqlDbType.Integer, id)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader (GetConnectionString (), CommandType.Text, _sqlGetById, param.ToArray ()))
                while (rdr.Read ())
                    entity = base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutColumnInfo> (rdr);

                return entity;
            });
        }

        protected override string GetSequenceName ()
        {
            return "SEQ_EDI_DETLAY";
        }

        public Task<List<LayoutColumnInfo>> GetByLayoutBandAsync(long layoutBandId)
        {
            return Task.Run(() =>
            {
                List<LayoutColumnInfo> list = new List<LayoutColumnInfo>();

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter (":DTL_BNL_IDENTI", NpgsqlDbType.Integer, layoutBandId)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetByLayoutBand, param.ToArray()))
                    while (rdr.Read())
                        list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutColumnInfo>(rdr));

                return list;
            });
        }

        public Task<List<LayoutColumnInfo>> GetByLayoutFileNameAsync(long layoutFileNameId) => Task.Run(() =>
        {
            List<LayoutColumnInfo> list = new List<LayoutColumnInfo>();

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter (":DTL_NAR_IDENTI", NpgsqlDbType.Integer, layoutFileNameId)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetByLayoutFileName, param.ToArray()))
                while (rdr.Read())
                    list.Add(base.PostgreSQLMapperBaseNew.NewClassInfo<LayoutColumnInfo>(rdr));

            return list;
        });
    }
}