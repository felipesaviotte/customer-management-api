using Confluent.Kafka;

namespace CustomerManagementApi.Infrastructure.Kafka;

/// <summary>
/// </summary>
public interface IKafkaProducer<TAvro>
{
    /// <summary>
    /// </summary>
    Task<DeliveryResult<string, TAvro>> ProduceAsync(string topicName, string key, TAvro value);
}
