using Istanbul.ApiIdempotency.Core;
using StackExchange.Redis;

namespace Istanbul.ApiIdempotency.Redis.StackExchange
{
    public class RedisApiIdempotencyDataStoreProvider : IApiIdempotencyDataStoreProvider
    {
        private readonly ConnectionMultiplexer _connectionMultiplexe;

        public RedisApiIdempotencyDataStoreProvider(ConnectionMultiplexer connectionMultiplexe)
        {
            _connectionMultiplexe = connectionMultiplexe;
        }

        public Task SetDataAsync(string key, string responseBody, int httpStatusCode, Dictionary<string, string> responseHeaders)
        {
            return Task.CompletedTask;
        }

        public Task<ApiIdempotencyResult> TryAcquireIdempotencyAsync(string key, int timeToLiveInMs)
        {
            return Task.FromResult(new ApiIdempotencyResult
            {
                IsIdempotencyAlreadyAcquired = false,
                HttpStatusCode = 201,
                ResponseBody = "{ \"TestData\": 123 }",
                ResponseHeaders = new Dictionary<string, string> { ["TestResponseHeaderKey"] = "data" }
            });
        }
    }
}