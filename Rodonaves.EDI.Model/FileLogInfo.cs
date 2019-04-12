using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Model
{
    public class FileLogInfo : BaseInfo
    {
        public class FileLogCollection : BaseCollection<FileLogInfo> { }

        public FileLogInfo()
        {
            Customer = new CustomerInfo();
            InitializePropertiesStates();
        }

        public string TransmitionView { get { return EDIEnumsList.GetSendingType().Find(p => p.Enum == Transmition).Description; } }

        public int Id { get; set; }

        public string Number { get; set; }

        public SendingTypeEnum Transmition { get; set; }

        public CustomerInfo Customer { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }
    }
}
