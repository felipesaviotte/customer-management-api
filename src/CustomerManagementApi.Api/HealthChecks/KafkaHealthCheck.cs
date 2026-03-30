using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CustomerManagementApi.Api.HealthChecks;

///<!---->
[ExcludeFromCodeCoverage]
public class KafkaHealthCheck(ProducerConfig producerConfig) : IHealthCheck
{
    private readonly ProducerConfig _producerConfig = producerConfig;

    ///<!---->
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            //TODO: Avaliar se é necessário criar um tópico específico para o health check ou se podemos usar um tópico existente. O ideal seria ter um tópico dedicado para evitar interferências com os tópicos de produção.
            //using var adminClient = new AdminClientBuilder(new AdminClientConfig
            //{
            //    BootstrapServers = _producerConfig.BootstrapServers,
            //    SecurityProtocol = _producerConfig.SecurityProtocol,
            //    SaslMechanism = _producerConfig.SaslMechanism,
            //    SaslUsername = _producerConfig.SaslUsername,
            //    SaslPassword = _producerConfig.SaslPassword,
            //    SslEndpointIdentificationAlgorithm = _producerConfig.SslEndpointIdentificationAlgorithm
            //}).Build();

            //adminClient.GetMetadata(TimeSpan.FromSeconds(5));

            return Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (KafkaException)
        {
            return Task.FromResult(HealthCheckResult.Degraded("Kafka cluster não respondeu dentro do tempo esperado."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Degraded(ex.Message));
        }
    }
}
