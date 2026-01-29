using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DSA.Api.Common.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDecorate<TInterface, TImplementation, TDecorator>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
            where TDecorator : class, TInterface
        {
            services.TryAddTransient<TImplementation>();
            services.AddTransient<TInterface>(sp =>
            {
                var impl = sp.GetRequiredService<TImplementation>();
                return ActivatorUtilities.CreateInstance<TDecorator>(sp, impl);
            });

            return services;
        }
    }
}
