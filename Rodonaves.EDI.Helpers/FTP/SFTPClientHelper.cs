using Renci.SshNet;
using Rodonaves.EDI.Helpers.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rodonaves.EDI.Helpers.FTP
{
    public class SftpClientHelper : FtpClientHelper
    {
        public SftpClientHelper(string address, string senderFolder, string receiverFolder, string user, string password, bool activeMode, int? port = 22) : base(address, senderFolder, receiverFolder, user, password, activeMode, port)
        {
        }

        public override void Update(string filePath)
        {
            using (var sftpClient = new SftpClient(_address, _user, _password))
            {
                sftpClient.Connect();

                using (var fs = new FileStream(filePath, FileMode.Open))
                {
                    sftpClient.UploadFile(fs, _senderFolder + "/" + Path.GetFileName(filePath));
                }

                sftpClient.Disconnect();
            }
        }

        public override FtpConnection TryConnection()
        {
            var connection = new FtpConnection();

            try
            {
                using (var sftpClient = new SftpClient(_address, _user, _password))
                {
                    sftpClient.Connect();
                    sftpClient.Disconnect();
                }
            }
            catch (Exception ex)
            {
                connection.IsValid = false;
                connection.ErrorMessage = ex.Message;
            }

            return connection;
        }
    }
}
