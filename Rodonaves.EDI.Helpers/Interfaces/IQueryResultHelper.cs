using Rodonaves.EDI.Helpers.DTO;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.Interfaces
{
    public interface IQueryResultHelper<T>
    {
        Task<QueryResult<T>> GetQueryResult(int page, int amountByPage);

        Task<QueryResult<T>> GetQueryResult();
    }
}
