using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class CommunicationChannelInfo : BaseInfo
    {
        public int Id { get; set; }

        public int GenerateReturnId { get; set; }

        public FtpInfo Ftp { get; set; } = new FtpInfo();

        public EmailInfo Email { get; set; } = new EmailInfo();

        public SendingTypeEnum SendingType { get; set; }

        public int AmountPages { get; set; }
    }
}
