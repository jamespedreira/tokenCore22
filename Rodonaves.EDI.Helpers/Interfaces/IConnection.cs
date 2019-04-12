using Rodonaves.EDI.Helpers.DTO;
using System;

namespace Rodonaves.EDI.Helpers.Interfaces
{
    public interface IConnection : IDisposable
    {
        ConnectionState TryConnection();
        void Open();
        void Close();
    }
}
