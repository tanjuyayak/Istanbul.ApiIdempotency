using Istanbul.ApiIdempotency.Core;

namespace Istanbul.ApiIdempotency
{
    public class ApiIdempotencyOptions
    {
        public IApiIdempotencyDataStoreProvider IdempotencyDataStoreProvider { get; set; }
        public string IdempotencyHeaderKey { get; set; }
    }
}
