using Oracle.ManagedDataAccess.Client;

namespace Rodonaves.EDI.Helpers.Oracle
{
    public class ConnectionHelper : ConnectionBase
    {
        public ConnectionHelper(string connectionString) : base(connectionString)
        {
            _connection = new OracleConnection(connectionString);
        }
    }
}
