using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class TriggerDal : BaseCrudDal<TriggerInfo>, ITriggerDal
    {
        #region SQL

        const string orderBy = @" ORDER BY CFG_IDENTI";

        const string _sqlGetAll = @"SELECT *
                                   FROM EDI_CONFIG ";

        const string _sqlGetById = _sqlGetAll +
            @" WHERE CFG_IDENTI = :CFG_IDENTI";

        const string _sqlGetByTaskId = _sqlGetAll +
            @" WHERE CFG_TRF_IDENTI = :CFG_TRF_IDENTI";

        #endregion

        #region Mapper
        readonly FieldsClass<TriggerInfo> fields = new FieldsClass<TriggerInfo> ();

        protected override PostgreSQLMapper<TriggerInfo> CreateMapperNew ()
        {
            var mapper = base.CreateMapperNew ();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof (TriggerInfo).Name;

            mapper.TableName = "EDI_CONFIG";

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Id), "CFG_IDENTI", false, null,
                ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Task.Id), "CFG_TRF_IDENTI", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Frequency), "CFG_FRQ_IDENTI", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Interval), "CFG_REPET", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.BeginDate), "CFG_DATINI", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.BeginTime), "CFG_HORINI", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.ExpireDate), "CFG_DATEXP", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.ExpireTime), "CFG_HOREXP", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Sunday), "CFG_DOMING", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Monday), "CFG_SEGUND", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Tuesday), "CFG_TERCA", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Wednesday), "CFG_QUARTA", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Thursday), "CFG_QUINTA", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Friday), "CFG_SEXTA", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Saturday), "CFG_SABADO", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Enable), "CFG_ATIVA", false, null,
                ORMapper.FieldFlags.InsertField, false);

            mapper.OnGetParameterValue += mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += mapper_OnSetInstanceValue;
            return mapper;
        }

        void mapper_OnGetParameterValue (ORLineMapper orLineMapper, object value, ref object newValue)
        {

            if (orLineMapper.PropertyName == fields.GetFieldNameFull (p => p.Frequency))
                newValue = (int) ((FrequencyType) value);
        }
        void mapper_OnSetInstanceValue (ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull (p => p.Frequency))
                newValue = (FrequencyType) value;
        }

        #endregion

        public Task<List<TriggerInfo>> GetAll ()
        {
            return Task.Run (() =>
            {
                var list = new List<TriggerInfo> ();

                using (var rdr = PostgreSQLHelper.ExecuteReader (GetConnectionString (), CommandType.Text, _sqlGetAll))
                while (rdr.Read ())
                    list.Add (base.PostgreSQLMapperBaseNew.NewClassInfo<TriggerInfo> (rdr));

                return list;
            });
        }

        public override Task<TriggerInfo> GetByIdAsync (int id)
        {
            return Task.Run (() =>
            {
                TriggerInfo entity = null;

                var param = new List<NpgsqlParameter> () { PostgreSQLHelper.CreateParameter (":CFG_IDENTI", NpgsqlDbType.Integer, id) };

                using (var rdr = PostgreSQLHelper.ExecuteReader (GetConnectionString (), CommandType.Text, _sqlGetById, param.ToArray ()))
                while (rdr.Read ())
                    entity = base.PostgreSQLMapperBaseNew.NewClassInfo<TriggerInfo> (rdr);

                return entity;
            });
        }

        public Task<List<TriggerInfo>> GetByParamsAsync (int id, int taskId, int? frequence, int page, int amountByPage)
        {
            return Task.Run (() =>
            {
                var list = new List<TriggerInfo> ();

                var pagination = new OraPaginationInfo
                {
                    AmountByPage = amountByPage,
                    Page = page,
                    FilterParameter = new StringBuilder ()
                };

                string sqlCommand = _sqlGetAll,
                    where = string.Empty;

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter> ();

                if (id != 0)
                {
                    DalUtils.SetFilterOnQuery ("CFG_IDENTI", ":ID", ref where);
                    parameters.Add (PostgreSQLHelper.CreateParameter (":ID", NpgsqlDbType.Integer, id));
                }

                if (taskId != 0)
                {
                    DalUtils.SetFilterOnQuery ("CFG_TRF_IDENTI", ":TASKID", ref where);
                    parameters.Add (PostgreSQLHelper.CreateParameter (":TASKID", NpgsqlDbType.Integer, id));
                }

                if (frequence.HasValue)
                {
                    DalUtils.SetFilterOnQuery ("CFG_FRQ_IDENTI", ":FREQUENCE", ref where);
                    parameters.Add (PostgreSQLHelper.CreateParameter (":FREQUENCE", NpgsqlDbType.Integer, frequence.GetValueOrDefault ()));
                }

                foreach (NpgsqlParameter item in parameters)
                    pagination.FilterParameter.Append (item.Value);

                sqlCommand += where + orderBy;

                int totalPages = PostgreSQLHelper.GetTotalPages (GetConnectionString (), parameters.ToArray (), sqlCommand, amountByPage);
                using (
                    var rdr = PostgreSQLHelper
                        .ExecuteReaderPagination (GetConnectionString (), CommandType.Text, sqlCommand, pagination, parameters.ToArray ())
                )
                while (rdr.Read ())
                {
                    var tskConfig = base.PostgreSQLMapperBaseNew.NewClassInfo<TriggerInfo> (rdr);
                    tskConfig.AmountPages = totalPages;

                    list.Add (tskConfig);
                }

                return list;
            });
        }

        public Task<List<TriggerInfo>> GetByTaskIdAsync (int taskId)
        {
            return Task.Run (() =>
            {
                List<TriggerInfo> entity = new List<TriggerInfo> ();

                var param = new List<NpgsqlParameter> () { PostgreSQLHelper.CreateParameter (":CFG_TRF_IDENTI", NpgsqlDbType.Integer, taskId) };

                using (var rdr = PostgreSQLHelper.ExecuteReader (GetConnectionString (), CommandType.Text, _sqlGetByTaskId, param.ToArray ()))
                while (rdr.Read ())
                {
                    try
                    {
                        entity.Add (base.PostgreSQLMapperBaseNew.NewClassInfo<TriggerInfo> (rdr));

                    }
                    catch (System.Exception exception)
                    {

                        throw new System.Exception (exception.ToString ());;
                    }
                }

                return entity;
            });
        }

        protected override string GetSequenceName ()
        {
            return "SEQ_CONFIG";
        }
    }
}