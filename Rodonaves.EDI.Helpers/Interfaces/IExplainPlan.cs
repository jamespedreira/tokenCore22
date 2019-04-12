using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.Interfaces
{
    public interface IExplainPlan
    {
        Task<string> getExplainPlan();
    }
}
