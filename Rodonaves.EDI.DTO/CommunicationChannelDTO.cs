using Rodonaves.EDI.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.DTO
{
    public class CommunicationChannelDTO : BaseDTO
    {
        public int GenerateReturnId { get; set; }

        public FtpDTO Ftp { get; set; } = new FtpDTO();

        public EmailDTO Email { get; set; } = new EmailDTO();

        public SendingTypeEnum SendingType { get; set; }
    }
}
