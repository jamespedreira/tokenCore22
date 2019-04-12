using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using System;
using System.Data.Common;

namespace Rodonaves.EDI.Helpers
{
    public class ConnectionBase : IConnection
    {
        protected DbConnection _connection;
        protected readonly string _connectionString;

        public ConnectionBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Open()
        {
            _connection.Open();
        }

        public ConnectionState TryConnection()
        {
            var _connectionState = new ConnectionState();

            try
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                {
                    this.Open();
                }
                _connectionState.IsConnected = true;
                _connectionState.State = System.Data.ConnectionState.Open;
            }
            catch (Exception ex)
            {
                _connectionState.IsConnected = false;
                _connectionState.ErrorMessage = ex.Message;
            }
            return _connectionState;
        }

        public void Close()
        {
            _connection.Close();
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                this.Close();
                _connection.Dispose();
            }
        }
    }
}
