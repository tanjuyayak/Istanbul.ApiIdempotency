namespace Istanbul.ApiIdempotency
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ApiIdempotencyAttribute : Attribute
    {
        private int _timeToLiveInMs;

        public ApiIdempotencyAttribute(int timeToLiveInMs)
        {
            _timeToLiveInMs = timeToLiveInMs;
        }

        public int GetTimeToLiveInMs() { return _timeToLiveInMs; }
    }
}