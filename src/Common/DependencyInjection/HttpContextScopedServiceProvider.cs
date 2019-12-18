using System;

using Microsoft.AspNetCore.Http;

namespace DataVault.Common.DependencyInjection
{
    /// <summary>
    /// A service provider that uses the current HttpContext request scope
    /// </summary>
    public class HttpContextScopedServiceProvider : IScopedServiceProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextScopedServiceProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            if (_httpContextAccessor.HttpContext == null)
                throw new Exception("Cannot resolve scoped service outside the context of an HTTP Request.");

            return _httpContextAccessor.HttpContext.RequestServices.GetService(serviceType);
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }
    }
}