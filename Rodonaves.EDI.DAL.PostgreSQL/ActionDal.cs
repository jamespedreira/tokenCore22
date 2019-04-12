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
    public class ActionDal : BaseCrudDal<ActionInfo>, IActionDal
    {
        #region SQL

        const string orderBy = @" ORDER BY ACA_IDENTI";

        const string _sqlGetByTaskId = @"SELECT *
                                               FROM EDI_ACAO 
                                               WHERE ACA_TRF_IDENTI = :ACA_TRF_IDENTI
                                               ORDER BY ACA_IDENTI";

        const string _sqlGetById = @"SELECT *
                                     FROM EDI_ACAO 
                                     WHERE ACA_IDENTI = :ACA_IDENTI";

        const string _sqlGetObjectsId = @"SELECT * 
                                          FROM EDI_ACAOBJ 
                                          WHERE AOB_ACA_IDENTI = :AOB_ACA_IDENTI";

        #endregion
        #region Mapper

        readonly FieldsClass<ActionInfo> fields = new FieldsClass<ActionInfo> ();

        protected override PostgreSQLMapper<ActionInfo> CreateMapperNew ()
        {
            var mapper = base.CreateMapperNew ();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof (ActionInfo).Name;

            mapper.TableName = "EDI_ACAO";
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.Id), "ACA_IDENTI", false, null,
                ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.KeyField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.Description), "ACA_DESCRI", false, null,
                ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.Task.Id), "ACA_TRF_IDENTI", false, null,
                ORMapper.FieldFlags.InsertField, false);

            return mapper;
        }
        #endregion
        public override Task<ActionInfo> GetByIdAsync (int id)
        {
            return Task.Run (() =>
            {
                ActionInfo entity = null;

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
                        entity = base.PostgreSQLMapperBaseNew.NewClassInfo<ActionInfo> (rdr);

                }

                return entity;
            });
        }

        public Task<List<ActionInfo>> GetByTaskIdAsync (int taskId)
        {
            return Task.Run (() =>
            {
                List<ActionInfo> list = new List<ActionInfo> ();

                var param = new List<NpgsqlParameter> () { PostgreSQLHelper.CreateParameter (":ACA_TRF_IDENTI", NpgsqlDbType.Integer, taskId) };

                using (var rdr = PostgreSQLHelper.ExecuteReader (GetConnectionString (), CommandType.Text, _sqlGetByTaskId, param.ToArray ()))
                while (rdr.Read ())
                    list.Add (base.PostgreSQLMapperBaseNew.NewClassInfo<ActionInfo> (rdr));

                return list;
            });
        }

        protected override string GetSequenceName ()
        {
            return "SEQ_ACAO";
        }
    }
}