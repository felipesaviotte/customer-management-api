using CustomerManagementApi.Domain.Entities;

namespace CustomerManagementApi.Application.Ports.Outbound.ExternalServices.Kafka;

/// <summary>
/// Produtor Kafka responsável por produzir eventos de link de pagamento.
/// </summary>
public interface ICustomerProducer
{
    /// <summary>
    /// Produz um evento de Customer para o tópico Kafka.
    /// </summary>
    public Task Produce();
}
