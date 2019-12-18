using System;
using System.Data;
using System.Data.Common;

namespace DataVault.Internal
{
    /// <summary>
    /// A fluent class for a data session.
    /// </summary>
    public class DataSession : DisposableBase, IDataSession
    {
        private readonly Action<string> _logger;
        private readonly bool _disposeConnection;

        private bool _openedConnection;
        private int _connectionRequestCount;

        public DbConnection Connection { get; }

        public DbTransaction Transaction { get; set; }

        public bool IsInTransaction => Transaction != null;

        public DataSession(DbConnection connection, bool disposeConnection = true, Action<string> logger = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (string.IsNullOrEmpty(connection.ConnectionString))
                throw new ArgumentException("Invalid connection string", nameof(connection));

            Connection = connection;
            _logger = logger;
            _disposeConnection = disposeConnection;
        }

        public DataSession(IDataConfiguration dataConfiguration)
        {
            if (dataConfiguration == null)
                throw new ArgumentNullException(nameof(dataConfiguration));

            Connection = dataConfiguration.CreateConnection();
            _logger = dataConfiguration.Logger;
            _disposeConnection = true;
        }

        public DbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            if (false == IsInTransaction)
            {
                EnsureConnection();
                Transaction = Connection.BeginTransaction(isolationLevel);
            }

            return Transaction;
        }

        public IDataCommand CreateCommand()
        {
            return new DataCommand(this, Transaction);
        }

        public IDataCommand Sql(string sql)
        {
            return CreateCommand().Sql(sql);
        }

        public IDataCommand StoredProcedure(string storedProcedureName)
        {
            return CreateCommand().StoredProcedure(storedProcedureName);
        }

        public void EnsureConnection()
        {
            AssertDisposed();

            if (ConnectionState.Closed == Connection.State)
            {
                Connection.Open();
                _openedConnection = true;
            }

            if (_openedConnection)
                _connectionRequestCount++;

            // Check the connection was opened correctly
            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                throw new InvalidOperationException($"Execution of the command requires an open and available connection. The connection's current state is {Connection.State}.");
        }

        public void ReleaseConnection()
        {
            AssertDisposed();

            if (!_openedConnection)
                return;

            if (_connectionRequestCount > 0)
                _connectionRequestCount--;

            if (_connectionRequestCount != 0)
                return;

            // When no operation is using the connection and the context had opened the connection
            // the connection can be closed
            Connection.Close();

            _openedConnection = false;
        }

        public void WriteLog(string message)
        {
            _logger?.Invoke(message);
        }

        protected override void DisposeManagedResources()
        {
            if (IsInTransaction)
            {
                //Transaction.Rollback();
                Transaction.Dispose();
                Transaction = null;
            }

            // Release managed resources here.
            if (Connection != null)
            {
                // Dispose the connection created

                if (_disposeConnection)
                {
                    // Force close connection
                    if (ConnectionState.Open == Connection.State)
                    {
                        Connection.Close();
                    }

                    Connection.Dispose();
                }
            }
        }
    }
}