namespace Istanbul.ApiIdempotency
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ApiIdempotencyAttribute : Attribute
    {
        private int _timeToLiveInSec;

        public ApiIdempotencyAttribute(int timeToLiveInSec)
        {
            _timeToLiveInSec = timeToLiveInSec;
        }

        public int GetTimeToLiveInSec() { return _timeToLiveInSec; }
    }
}