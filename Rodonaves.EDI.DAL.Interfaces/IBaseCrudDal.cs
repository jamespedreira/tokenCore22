using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IBaseCrudDal<T> : IDal where T : BaseInfo
    {
        Task<T> GetByIdAsync(int id);
        Task<bool> InsertAsync(T entity);
        Task<bool> InsertAsync(T entity, IDbTransaction trans);
        Task<bool> UpdateAsync(T entity);
        Task<bool> UpdateAsync(T entity, IDbTransaction trans);
        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteAsync(T entity, IDbTransaction trans);
    }
}
