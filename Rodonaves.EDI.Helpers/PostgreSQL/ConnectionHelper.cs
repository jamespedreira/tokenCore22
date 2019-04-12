using Npgsql;
using Rodonaves.EDI.Helpers.Interfaces;

namespace Rodonaves.EDI.Helpers.PostgreSQL
{
    public class ConnectionHelper : ConnectionBase
    {
        public ConnectionHelper(string connectionString) : base(connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }
    }
}
