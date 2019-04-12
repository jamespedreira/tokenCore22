using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Model
{
    public class ExportInfo
    {
        public string ColumnName { get; set; }
        public string ColumnDescription { get { return this.ColumnName.Replace(":", string.Empty); } }
        public string ColumnValue { get; set; }
        public string TableName { get; set; }
        public string TableColumn { get; set; }
        public string Condition { get; set; }
    }
}
