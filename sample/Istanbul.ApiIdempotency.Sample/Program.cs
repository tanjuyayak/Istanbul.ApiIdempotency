using Istanbul.ApiIdempotency;
using Istanbul.ApiIdempotency.Redis.StackExchange;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var redisConnectionMultiplexer = ConnectionMultiplexer.Connect("localhost");

builder.Services.AddApiIdempotency(options =>
{
    options.IdempotencyHeaderKey = "X-Idempotency-Key";
    options.IdempotencyDataStoreProvider = new RedisApiIdempotencyDataStoreProvider(redisConnectionMultiplexer);
});

var app = builder.Build();

app.UseApiIdempotency();

app.MapControllers();

app.Run();
