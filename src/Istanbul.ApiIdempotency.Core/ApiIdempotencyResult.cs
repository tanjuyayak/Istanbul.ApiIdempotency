namespace Istanbul.ApiIdempotency.Core
{
    public class ApiIdempotencyResult
    {
        public bool KeyExists { get; init; }
        public ApiResponseCache ResponseCache { get; init; }
    }
}
