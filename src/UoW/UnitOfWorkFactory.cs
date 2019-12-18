using Microsoft.Extensions.DependencyInjection;

namespace DataVault.UoW
{
    public class UnitOfWorkFactory
    {
        public static IUnitOfWork CreateUnitOfWork()
        {
            // build the service collection
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataVault();

            // extract the provider instance from the service collection
            return serviceCollection.BuildServiceProvider().GetRequiredService<IUnitOfWork>();
        }
    }
}