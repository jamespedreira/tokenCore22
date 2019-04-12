using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Requests
{

    public class FTPRequest
    {
        public string Address { get; set; }

        public string SenderFolder { get; set; }

        public string ReceiverFolder { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public bool ActiveMode { get; set; }

        public bool EnableSSH { get; set; }
    }
}
