using System.Net.Http;

namespace Rodonaves.EDI.Helpers.Interfaces
{
    public interface IValidateClientAuthentication
    {
        HttpClient CreateClient(string Route);
    }
}
