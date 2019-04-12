using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.Helpers.DTO;
using System;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class Connection : IConnection
    {
        IFtpClientFactory _FtpClientFactory;

        public Connection(IFtpClientFactory FTPClientFactory)
        {
            _FtpClientFactory = FTPClientFactory;
        }

        public async Task<FtpConnection> TestFtpConnection(string address, string senderFolder, string receiverFolder, string user, string password, int port, bool activeMode, bool enableSSH)
        {
            var client = _FtpClientFactory.CreateNew(address, senderFolder, receiverFolder, user, password, activeMode, enableSSH);

            return client.TryConnection();
        }
    }
}
