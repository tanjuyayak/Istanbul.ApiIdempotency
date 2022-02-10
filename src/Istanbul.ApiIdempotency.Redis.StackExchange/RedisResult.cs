namespace Istanbul.ApiIdempotency.Redis.StackExchange
{
    public class RedisResult
    {
        public bool KeyExists { get; init; }
        public RedisResponseCache Data { get; init; }
    }
}