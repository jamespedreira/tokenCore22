using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Rodonaves.Core.Security;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.Engine;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public abstract class BaseCrudDal<T> : PostgreSQLDal<T>, IBaseCrudDal<T> where T : BaseInfo
    {
        protected BaseCrudDal ()
        {
            base.classFields = new FieldsClass<T> ();
        }
        protected string className = typeof (T).Name;

        protected override string GetConnectionString ()
        {
            return Global.GetConnectionString ("RTEEDI");
        }

        public Task<bool> InsertAsync (T entity)
        {
            return Task.Run (() =>
            {
                return PostgreSQLMapperBaseNew.Insert (entity,
                    GetConnectionString (),
                    GetType ().Name,
                    SecurityPermission.GetAuthenticatedUser ().UserInfo.Id);
            });
        }

        public Task<bool> InsertAsync (T entity, IDbTransaction trans)
        {
            return Task.Run (() =>
            {
                return PostgreSQLMapperBaseNew.Insert (entity,
                    trans,
                    GetType ().Name,
                    SecurityPermission.GetAuthenticatedUser ().UserInfo.Id);
            });
        }

        public Task<bool> UpdateAsync (T entity)
        {
            return Task.Run (() =>
            {
                return PostgreSQLMapperBaseNew.Update (entity,
                    GetConnectionString (),
                    GetType ().Name,
                    SecurityPermission.GetAuthenticatedUser ().UserInfo.Id);
            });
        }
        public Task<bool> UpdateAsync (T entity, IDbTransaction trans)
        {
            return Task.Run (() =>
            {
                return PostgreSQLMapperBaseNew.Update (entity,
                    trans,
                    GetType ().Name,
                    SecurityPermission.GetAuthenticatedUser ().UserInfo.Id);
            });
        }

        public Task<bool> DeleteAsync (T entity)
        {
            PostgreSQLMapperBaseNew.GetDeleteCommand ();
            return Task.Run (() =>
            {
                return PostgreSQLMapperBaseNew.Delete (entity,
                    GetConnectionString ());
            });
        }

        public Task<bool> DeleteAsync (T entity, IDbTransaction trans)
        {
            return Task.Run (() =>
            {
                return PostgreSQLMapperBaseNew.Delete (entity,
                    trans);
            });
        }

        public abstract Task<T> GetByIdAsync (int id);
    }
}