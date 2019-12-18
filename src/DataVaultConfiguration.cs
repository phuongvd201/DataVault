using System;

using DataVault.Internal;

using MySql.Data.MySqlClient;

namespace DataVault
{
    public class DataVaultConfiguration : DataConfiguration, IDataVaultConfiguration
    {
        public DataVaultConfiguration(IConnectionStringProvider connectionStringProvider)
            : base(
                MySqlClientFactory.Instance,
                new Lazy<string>(connectionStringProvider.GetConnectionString),
                Console.WriteLine)
        {
        }
    }
}