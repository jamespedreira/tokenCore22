using Rodonaves.EDI.DTO;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IBaseCrud<T> where T : BaseDTO
    {
        Task<int> InsertAsync(T dto);
        Task<bool> UpdateAsync(T dto);
        Task<bool> DeleteAsync(int id);
    }
}
