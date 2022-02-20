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

            var options = new ApiIdempotencyOptions();
            idempotencyOptions(options);

            if (string.IsNullOrWhiteSpace(options.IdempotencyHeaderKey))
            {
                throw new ArgumentException($"{nameof(options.IdempotencyHeaderKey)} is NULL or empty or contains only white space(s)");
            }

            if (options.IdempotencyDataStoreProvider == null)
            {
                throw new ArgumentNullException(nameof(options.IdempotencyDataStoreProvider));
            }

            var internalOptions = (ApiIdempotencyInternalOptions data) =>
            {
                data.IdempotencyHeaderKey = options.IdempotencyHeaderKey;
            };

            services.Configure(internalOptions);

            services.AddSingleton(serviceProvider => { return options.IdempotencyDataStoreProvider; });

            return services;
        }
    }
}