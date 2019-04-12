using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.Interfaces;

namespace Rodonaves.EDI.BLL.Interfaces.Factories
{
    public interface IGedFactory
    {
        IGed CreateNew(ProviderEnum provider);
    }
}
