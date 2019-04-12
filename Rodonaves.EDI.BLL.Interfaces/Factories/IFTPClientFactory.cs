using Rodonaves.EDI.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.BLL.Interfaces.Factories
{
    public interface IFtpClientFactory
    {
        IftpClientHelper CreateNew(string address, string senderFolder, string receiverFolder, string user, string password, bool activeMode, bool enableSSH);

        IftpClientHelper CreateNew(string address, string senderFolder, string receiverFolder, string user, string password, bool activeMode, bool enableSSH, int port);
    }
}
