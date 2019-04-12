using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Exceptions;
using Rodonaves.EDI.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Rodonaves.EDI.Helpers.FTP
{
    public class FtpClientHelper : IftpClientHelper
    {
        protected string _address { get; set; }
        protected string _senderFolder { get; set; }
        protected string _receiverFolder { get; set; }
        protected string _user { get; set; }
        protected string _password { get; set; }
        protected string _proccessFolder { get; set; }
        protected int _port { get; set; }
        protected bool _activeMode { get; set; }

        public FtpClientHelper(string address, string senderFolder, string receiverFolder, string user, string password, bool activeMode, int? port = 21)
        {
            _address = address;
            _senderFolder = senderFolder;
            _receiverFolder = receiverFolder;
            _user = user;
            _password = password;
            _port = port.GetValueOrDefault();
            _activeMode = activeMode;
        }

        public virtual void Update(string filePath)
        {
            var request = CreateFtpRequest(filePath);

            using (var sourceStream = new StreamReader(filePath))
            {
                var fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;

                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                }

                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    response.Close();
                }
            }
        }

        public virtual FtpConnection TryConnection()
        {
            var connection = new FtpConnection();

            try
            {
                var testFile = $"test_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.txt";

                var request = CreateFtpRequest(testFile);

                using (var requestStream = request.GetRequestStream())
                {

                    if (!requestStream.CanWrite)
                    {
                        throw new FTPException("Não há permissão de escrita no diretório configurado");
                    }

                    requestStream.Close();

                    connection.IsValid = true;
                }

                var requestDelete = CreateFtpRequest(testFile, WebRequestMethods.Ftp.DeleteFile);

                using (var response = requestDelete.GetResponse())
                    response.Close();
            }
            catch (Exception ex)
            {
                connection.IsValid = false;
                connection.ErrorMessage = ex.Message;
            }

            return connection;
        }

        private FtpWebRequest CreateFtpRequest(string filePath = "", string method = WebRequestMethods.Ftp.UploadFile)
        {
            string fileName = !string.IsNullOrEmpty(filePath) ? $"/{Path.GetFileName(filePath)}" : string.Empty;

            string senderFolder = !string.IsNullOrEmpty(_senderFolder) ? $"/{_senderFolder}" : string.Empty;

            var request = (FtpWebRequest)WebRequest.Create($"{this._address}{senderFolder}{fileName}");
            request.Method = method;
            request.Credentials = new NetworkCredential(this._user, this._password);
            request.UsePassive = !this._activeMode;

            return request;
        }
    }
}
