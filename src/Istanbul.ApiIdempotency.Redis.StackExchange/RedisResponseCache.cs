namespace Istanbul.ApiIdempotency.Redis.StackExchange
{
    public class RedisResponseCache
    {
        public string ResponseBody { get; init; }
        public int HttpStatusCode { get; init; }
        public Dictionary<string, string> ResponseHeaders { get; init; }
    }
}
