using System;
using System.Data.Common;

namespace DataVault.Internal
{
    public interface IDataConfiguration
    {
        DbProviderFactory ProviderFactory { get; }

        string ConnectionString { get; }

        Action<string> Logger { get; }

        IDataSession CreateSession();

        DbConnection CreateConnection();
    }
}