using System;
using System.Data.Common;

namespace DataVault.Internal
{
    /// <summary>
    /// The database configuration
    /// </summary>
    /// <seealso cref="IDataConfiguration" />
    public class DataConfiguration : IDataConfiguration
    {
        private readonly Lazy<string> _connectionString;
        private readonly Lazy<DbProviderFactory> _dbProviderFactory;

        public DataConfiguration(DbProviderFactory providerFactory, Lazy<string> connectionString, Action<string> logger = null)
        {
            _connectionString = connectionString;
            _dbProviderFactory = new Lazy<DbProviderFactory>(providerFactory);
            Logger = logger;
        }

        public virtual DbProviderFactory ProviderFactory => _dbProviderFactory.Value;

        public virtual string ConnectionString => _connectionString.Value;

        public virtual Action<string> Logger { get; }

        public virtual IDataSession CreateSession()
        {
            var connection = CreateConnection();
            var dataSession = new DataSession(connection, true, Logger);
            return dataSession;
        }

        public virtual DbConnection CreateConnection()
        {
            var connection = ProviderFactory.CreateConnection();
            if (connection == null)
                throw new InvalidOperationException("Database provider factory failed to create a connection object.");

            if (string.IsNullOrEmpty(ConnectionString))
                throw new InvalidOperationException("The connection string is invalid");

            connection.ConnectionString = ConnectionString;
            return connection;
        }
    }
}