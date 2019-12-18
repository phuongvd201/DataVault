namespace DataVault.Internal
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString();

        string GetConnectionString(string databaseName);
    }
}