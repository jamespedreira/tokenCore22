using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.DTO
{
    public class FtpDTO
    {
        public bool EnableSSH { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Address { get; set; }

        public string SenderFolder { get; set; }

        public string ReceiverFolder { get; set; }

        public bool ActiveMode { get; set; }
    }
}
