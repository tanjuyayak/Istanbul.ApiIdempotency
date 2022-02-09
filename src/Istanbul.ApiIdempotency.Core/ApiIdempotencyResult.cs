namespace Istanbul.ApiIdempotency.Core
{
    public class ApiIdempotencyResult
    {
        public bool IsIdempotencyAlreadyAcquired { get; init; }
        public string ResponseData { get; init; }
        public Dictionary<string, string> ResponseHeaders { get; init; }
    }
}
