using Istanbul.ApiIdempotency.Core;

namespace Istanbul.ApiIdempotency.Redis.StackExchange
{
    public class RedisIdempotencyDataStoreProvider : IIdempotencyDataStoreProvider
    {
        public async Task SetDataAsync(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<IdempotencyResult> TryAcquireIdempotencyAsync(string key, int timeToLiveInMs)
        {
            throw new NotImplementedException();
        }
    }
}