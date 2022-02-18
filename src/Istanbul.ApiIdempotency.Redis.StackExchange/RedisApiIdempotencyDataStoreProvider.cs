using Istanbul.ApiIdempotency.Core;
using StackExchange.Redis;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Istanbul.ApiIdempotency.Redis.StackExchange
{
    public class RedisApiIdempotencyDataStoreProvider : IApiIdempotencyDataStoreProvider
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisApiIdempotencyDataStoreProvider(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task SetDataAsync(string key, int timeToLiveInSec, string responseBody, int httpStatusCode, Dictionary<string, string> responseHeaders)
        {
            var responseCache = new RedisResponseCache
            {
                ResponseBody = string.IsNullOrWhiteSpace(responseBody) ? responseBody : Base64Encode(responseBody),
                HttpStatusCode = httpStatusCode,
                ResponseHeaders = responseHeaders
            };

            var jsonResponseCache = JsonSerializer.Serialize(responseCache, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            var db = _connectionMultiplexer.GetDatabase(0);
            await db.StringSetAsync(key, jsonResponseCache, TimeSpan.FromSeconds(timeToLiveInSec));
        }

        public async Task<ApiIdempotencyResult> TryCheckKeyExistsAsync(string key, int timeToLiveInSec)
        {
            const string luaScript = "if redis.call('SETNX', KEYS[1], 'null') == 0 then return '{ \"KeyExists\": true, \"Data\": '..redis.call('GET', KEYS[1])..'}' else redis.call('EXPIRE', KEYS[1], ARGV[1]) return '{ \"KeyExists\": false }' end";
            var db = _connectionMultiplexer.GetDatabase(0);
            
            var redisResult = await db.ScriptEvaluateAsync(luaScript, new RedisKey[] { key }, new RedisValue[] { timeToLiveInSec });

            var jsonRedisResult = redisResult?.ToString();

            if (string.IsNullOrWhiteSpace(jsonRedisResult))
            {
                throw new Exception("Result received from Redis is null, empty or contains only white spaces!");
            }

            var objectRedisResult = JsonSerializer.Deserialize<RedisResult>(jsonRedisResult);
            if (objectRedisResult == null)
            {
                throw new Exception("Result received from Redis not compatible with expected format!");
            }

            var responseCache = objectRedisResult.Data != null ? new ApiResponseCache
            {
                ResponseBody = string.IsNullOrWhiteSpace(objectRedisResult.Data.ResponseBody) ? objectRedisResult.Data.ResponseBody : Base64Decode(objectRedisResult.Data.ResponseBody),
                HttpStatusCode = objectRedisResult.Data.HttpStatusCode,
                ResponseHeaders = objectRedisResult.Data.ResponseHeaders
            } : null;

            return new ApiIdempotencyResult
            {
                KeyExists = objectRedisResult.KeyExists,
                ResponseCache = responseCache
            };
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}