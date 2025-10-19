using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace ProductService.API.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisHealthCheck(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_redis.IsConnected)
                    return HealthCheckResult.Unhealthy("Multiplexer no conectado.");

                var db = _redis.GetDatabase();
                var latency = await db.PingAsync();

                return latency < TimeSpan.FromSeconds(2)
                    ? HealthCheckResult.Healthy($"Redis respondió en {latency.TotalMilliseconds} ms")
                    : HealthCheckResult.Degraded($"Redis respondió lento: {latency.TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Error al conectar con Redis", ex);
            }
        }
    }
}