using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class TaskDal : BaseCrudDal<TaskInfo>, ITaskDal
    {
        #region SQL

        const string orderBy = @" ORDER BY TRF_IDENTI";

        const string _sqlGetAll = @"SELECT *
                                   FROM EDI_TAREFA ";

        const string _sqlGetById = _sqlGetAll +
            @" WHERE TRF_IDENTI = :TRF_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<TaskInfo> fields = new FieldsClass<TaskInfo> ();

        protected override PostgreSQLMapper<TaskInfo> CreateMapperNew ()
        {
            var mapper = base.CreateMapperNew ();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof (TaskInfo).Name;
            mapper.TableName = "EDI_TAREFA";
            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Id),
                "TRF_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Name),
                "TRF_NOME", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper (className, fields.GetFieldNameFull (p => p.Description),
                "TRF_DESCRI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            return mapper;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName ()
        {
            return "SEQ_TAREF";
        }

        #endregion

        #region Methods

        Task<List<TaskInfo>> ITaskDal.GetAll ()
        {
            return Task.Run (() =>
            {
                var list = new List<TaskInfo> ();

                using (var rdr = PostgreSQLHelper.ExecuteReader (GetConnectionString (), CommandType.Text, _sqlGetAll))
                while (rdr.Read ())
                    list.Add (base.PostgreSQLMapperBaseNew.NewClassInfo<TaskInfo> (rdr));

                return list;
            });
        }

        public Task<List<TaskInfo>> GetByParamsAsync (int id, string name, string descripton, int page, int amountByPage)
        {

            return Task.Run (() =>
            {
                var list = new List<TaskInfo> ();

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
                    DalUtils.SetFilterOnQuery ("TRF_IDENTI", ":ID", ref where);
                    parameters.Add (PostgreSQLHelper.CreateParameter (":ID", NpgsqlDbType.Integer, id));
                }

                if (!string.IsNullOrEmpty (name))
                {
                    DalUtils.SetCustomFilterOnQuery ("TRF_NOME LIKE :NAME", ref where);
                    parameters.Add (PostgreSQLHelper.CreateParameter (":NAME", NpgsqlDbType.Varchar, "%" + name + "%"));
                }

                if (!string.IsNullOrEmpty (descripton))
                {
                    DalUtils.SetCustomFilterOnQuery ("TRF_DESCRI LIKE :DESCRIPTION", ref where);
                    parameters.Add (PostgreSQLHelper.CreateParameter (":DESCRIPTION", NpgsqlDbType.Varchar, "%" + descripton + "%"));
                }

                foreach (NpgsqlParameter item in parameters)
                    pagination.FilterParameter.Append (item.Value);

                sqlCommand += where + orderBy;

                int totalPages = PostgreSQLHelper.GetTotalPages (GetConnectionString (), parameters.ToArray (), sqlCommand, amountByPage);
                using (var rdr = PostgreSQLHelper.ExecuteReaderPagination (GetConnectionString (), CommandType.Text, sqlCommand, pagination, parameters.ToArray ()))
                while (rdr.Read ())
                {
                    var task = base.PostgreSQLMapperBaseNew.NewClassInfo<TaskInfo> (rdr);
                    task.AmountPages = totalPages;

                    list.Add (task);
                }

                return list;
            });
        }

        public override Task<TaskInfo> GetByIdAsync (int id)
        {
            return Task.Run (() =>
            {
                TaskInfo entity = null;

                var param = new List<NpgsqlParameter> () { PostgreSQLHelper.CreateParameter (":TRF_IDENTI", NpgsqlDbType.Integer, id) };

                using (var rdr = PostgreSQLHelper.ExecuteReader (GetConnectionString (), CommandType.Text, _sqlGetById, param.ToArray ()))
                while (rdr.Read ())
                    entity = base.PostgreSQLMapperBaseNew.NewClassInfo<TaskInfo> (rdr);

                return entity;
            });
        }
        #endregion
    }
}