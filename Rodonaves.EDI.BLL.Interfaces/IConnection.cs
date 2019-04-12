using Rodonaves.EDI.Helpers.DTO;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL.Interfaces
{
    public interface IConnection
    {
        Task<FtpConnection> TestFtpConnection(string address, string senderFolder, string receiverFolder, string user, string password, int port, bool activeMode, bool enableSSH);
    }
}
