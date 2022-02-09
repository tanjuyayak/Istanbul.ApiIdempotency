using Istanbul.ApiIdempotency;
using Istanbul.ApiIdempotency.Redis.StackExchange;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//var redisConnectionMultiplexer = ConnectionMultiplexer.Connect("localhost");

builder.Services.AddApiIdempotency(options =>
{
    options.IdempotencyHeaderKey = "X-Idempotency-Key";
    options.IdempotencyDataStoreProvider = new RedisApiIdempotencyDataStoreProvider(null);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseApiIdempotency();

app.UseAuthorization();

app.MapControllers();

app.Run();
