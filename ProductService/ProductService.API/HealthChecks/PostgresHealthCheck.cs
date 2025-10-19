using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace ProductService.API.HealthChecks
{
    public class PostgresHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public PostgresHealthCheck(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                await using var cmd = new NpgsqlCommand("SELECT 1", connection);
                var result = await cmd.ExecuteScalarAsync(cancellationToken);

                return result?.ToString() == "1"
                    ? HealthCheckResult.Healthy("PostgreSQL respondió correctamente.")
                    : HealthCheckResult.Unhealthy("PostgreSQL no devolvió el valor esperado.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Error al conectar con PostgreSQL", ex);
            }
        }
    }
}