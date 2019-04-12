using System;
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
    public class ActionObjectDal : BaseCrudDal<ActionObjectInfo>, IActionObjectDal
    {
        #region SQL
        const string _sqlGetByActionId =
            @" SELECT * FROM EDI_ACAOBJ WHERE AOB_ACA_IDENTI = :AOB_ACA_IDENTI";
        const string _delete =
            @" DELETE FROM EDI_ACAOBJ WHERE AOB_ACA_IDENTI = :AOB_ACA_IDENTI";

        #endregion
        #region Mapper

        readonly FieldsClass<ActionObjectInfo> fields = new FieldsClass<ActionObjectInfo> ();

        protected override PostgreSQLMapper<ActionObjectInfo> CreateMapperNew ()
        {
            var mapper = base.CreateMapperNew ();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof (ActionObjectInfo).Name;

            mapper.TableName = "EDI_ACAOBJ";
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.Id), "AOB_IDENTI", false, null,
                ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.KeyField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.ActionId), "AOB_ACA_IDENTI", false, null,
                ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.KeyField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.ObjectId), "AOB_OBJ_IDENTI", false, null,
                ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.Arguments), "AOB_ARGUME", false, null,
                ORMapper.FieldFlags.InsertField, false);

            return mapper;
        }
        #endregion

        public override Task<ActionObjectInfo> GetByIdAsync (int id)
        {
            throw new NotImplementedException ();
        }
        protected override string GetSequenceName () => "SEQ_AOB";

        public Task<List<ActionObjectInfo>> GetByActionIdAsync (int actionId)
        {
            return Task.Run (() =>
            {
                List<ActionObjectInfo> list = new List<ActionObjectInfo> ();

                var param = new List<NpgsqlParameter> ()
                {
                    PostgreSQLHelper.CreateParameter (":AOB_ACA_IDENTI", NpgsqlDbType.Integer, actionId)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader (
                    GetConnectionString (),
                    CommandType.Text,
                    _sqlGetByActionId,
                    param.ToArray ()))
                {
                    while (rdr.Read ())
                    {
                        list.Add (base.PostgreSQLMapperBaseNew.NewClassInfo<ActionObjectInfo> (rdr));
                    }

                }

                return list;
            });
        }

        public Task<bool> DeleteByActionAsync (int actionId) => Task.Run (() =>
        {
            var param = new List<NpgsqlParameter> ()
            {
            PostgreSQLHelper.CreateParameter (":AOB_ACA_IDENTI", NpgsqlDbType.Integer, actionId)
            };

            try
            {
                var rdr = PostgreSQLHelper.ExecuteNonQuery (
                    GetConnectionString (),
                    CommandType.Text,
                    _delete,
                    param.ToArray ()
                );

                return rdr >= 0;
            }
            catch (System.Exception exception)
            {

                throw new System.Exception (exception.ToString ());
            }

        });

    }
}