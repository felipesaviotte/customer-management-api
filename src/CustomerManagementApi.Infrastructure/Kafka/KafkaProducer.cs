using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using CustomerManagementApi.Application.Logs;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Infrastructure.Kafka;

/// <summary>
/// Produtor genérico do Kafka para enviar mensagens com serialização Avro.
/// </summary>
/// <typeparam name="TAvro">O tipo do objeto Avro a ser serializado e enviado para o Kafka.</typeparam>
[ExcludeFromCodeCoverage]
public class KafkaProducer<TAvro>(ProducerConfig producerConfig, ISchemaRegistryClient schemaRegistryClient, ILoggingService logger) : IKafkaProducer<TAvro>
{
    private readonly IProducer<string, TAvro> Producer = new ProducerBuilder<string, TAvro>(producerConfig)
                                                            .SetValueSerializer(new AvroSerializer<TAvro>(schemaRegistryClient))
                                                            .SetLogHandler((_, logMessage) => 
                                                            {
                                                                if (logMessage.Level <= SyslogLevel.Error)
                                                                    logger.Error("KafkaError", new Exception(logMessage.Message), logMessage.Message);
                                                            })
                                                            .Build();

    /// <summary>
    /// Produz uma mensagem para o tópico especificado no Kafka.
    /// </summary>
    /// <param name="topicName">O nome do tópico do Kafka onde a mensagem será enviada.</param>
    /// <param name="key">A chave da mensagem, usada para particionamento no Kafka.</param>
    /// <param name="value">O valor da mensagem, que será serializado usando Avro.</param>
    /// <returns>Um <see cref="DeliveryResult{TKey, TValue}"/> contendo o resultado da entrega da mensagem.</returns>
    public async Task<DeliveryResult<string, TAvro>> ProduceAsync(string topicName, string key, TAvro value)
    {
        return await Producer.ProduceAsync(topicName, new Message<string, TAvro>
        {
            Key = key,
            Value = value
        });
    }
}
