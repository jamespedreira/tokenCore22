using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Helpers.DTO
{
    public class ConnectionState
    {
        public bool IsConnected { get; set; }
        public System.Data.ConnectionState State { get; set; }
        public string ErrorMessage { get; set; }
    }
}
