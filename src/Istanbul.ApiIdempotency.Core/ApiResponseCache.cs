namespace Istanbul.ApiIdempotency.Core
{
    public class ApiResponseCache
    {
        public string ResponseBody { get; init; }
        public int HttpStatusCode { get; init; }
        public Dictionary<string, string> ResponseHeaders { get; init; }
    }
}
