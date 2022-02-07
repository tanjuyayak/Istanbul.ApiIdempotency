using Microsoft.Extensions.DependencyInjection;

namespace Istanbul.ApiIdempotency
{
    public static class ApiIdempotencyServiceCollectionExtensions
    {
        public static IServiceCollection AddApiIdempotency(this IServiceCollection services, Action<ApiIdempotencyOptions> idempotencyOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (idempotencyOptions == null)
            {
                throw new ArgumentNullException(nameof(idempotencyOptions));
            }

            services.Configure(idempotencyOptions);
            return services;
        }
    }
}