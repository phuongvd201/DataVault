using System;

namespace DataVault.Common.DependencyInjection
{
    public interface IScopedServiceProvider : IServiceProvider
    {
        T GetService<T>();
    }
}