using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.DAL.PostgreSQL.Util
{
    public static class DalUtils
    {
        public static void SetFilterOnQuery(string column, string parameter, ref string where)
        {
            where += string.IsNullOrEmpty(where) ? " WHERE " : " AND ";

            where += string.Format(" {0} = {1} ", column, parameter);
        }

        public static  void SetCustomFilterOnQuery(string filter, ref string where)
        {
            where += string.IsNullOrEmpty(where) ? " WHERE " : " AND ";
            where += filter;
        }
    }
}
