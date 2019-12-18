using DataVault.Common.Caching;
using DataVault.Entities.Audit;
using DataVault.Internal;
using DataVault.UoW;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DataVault
{
    public static class DataVaultServiceCollectionExtensions
    {
        public static IServiceCollection AddDataVault(this IServiceCollection services)
        {
#if DEBUG
            services.TryAddSingleton<IConnectionStringProvider, LocalConnectionStringProvider>();

#else
            services.TryAddSingleton<IConnectionStringProvider, ParameterStoreConnectionStringProvider>();
            services.AddHttpContextAccessor();
#endif

            services.TryAddSingleton<IAuditContext, AuditContext>();
            services.TryAddSingleton<IDataVaultConfiguration, DataVaultConfiguration>();

            services.TryAddScoped<IDataVaultContext, DataVaultContext>();
            services.TryAddScoped<IUnitOfWork, UnitOfWork>();

            services.AddCaching();

            return services;
        }
    }
}