namespace Istanbul.ApiIdempotency.Core
{
    public class ApiIdempotencyResult
    {
        public bool IsIdempotencyAlreadyAcquired { get; init; }
        public string JsonResponseData { get; init; }
        public int HttpStatusCode { get; init; }
        public Dictionary<string, string> ResponseHeaders { get; init; }
    }
}
