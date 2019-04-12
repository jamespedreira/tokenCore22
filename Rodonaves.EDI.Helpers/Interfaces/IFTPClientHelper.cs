using Rodonaves.EDI.Helpers.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Helpers.Interfaces
{
    public interface IftpClientHelper
    {
        void Update(string filePath);

        FtpConnection TryConnection();
    }
}
