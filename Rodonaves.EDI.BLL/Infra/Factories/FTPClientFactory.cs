using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.Helpers.FTP;
using Rodonaves.EDI.Helpers.Interfaces;

namespace Rodonaves.EDI.BLL.Infra.Factories
{
    public class FtpClientFactory : IFtpClientFactory
    {
        public IftpClientHelper CreateNew(string address, string senderFolder, string receiverFolder, string user, string password, bool activeMode, bool enableSSH)
        {
            if (!enableSSH)
                return new FtpClientHelper(address, senderFolder, receiverFolder, user, password, activeMode);
            else
                return new SftpClientHelper(address, senderFolder, receiverFolder, user, password, activeMode);
        }

        public IftpClientHelper CreateNew(string address, string senderFolder, string receiverFolder, string user, string password, bool activeMode, bool enableSSH, int port)
        {
            if (!enableSSH)
                return new FtpClientHelper(address, senderFolder, receiverFolder, user, password, activeMode, port);
            else
                return new SftpClientHelper(address, senderFolder, receiverFolder, user, password, activeMode, port);
        }
    }
}
