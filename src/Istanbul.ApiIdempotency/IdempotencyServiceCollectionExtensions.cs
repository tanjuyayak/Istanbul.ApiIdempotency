using Microsoft.Extensions.DependencyInjection;

namespace Istanbul.ApiIdempotency
{
    public static class IdempotencyServiceCollectionExtensions
    {
        public static IServiceCollection AddIdempotency(this IServiceCollection services, Action<IdempotencyOptions> idempotencyOptions)
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