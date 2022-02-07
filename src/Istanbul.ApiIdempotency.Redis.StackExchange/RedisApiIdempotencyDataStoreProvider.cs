using Istanbul.ApiIdempotency.Core;

namespace Istanbul.ApiIdempotency.Redis.StackExchange
{
    public class RedisApiIdempotencyDataStoreProvider : IApiIdempotencyDataStoreProvider
    {
        public async Task SetDataAsync(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiIdempotencyResult> TryAcquireIdempotencyAsync(string key, int timeToLiveInMs)
        {
            throw new NotImplementedException();
        }
    }
}