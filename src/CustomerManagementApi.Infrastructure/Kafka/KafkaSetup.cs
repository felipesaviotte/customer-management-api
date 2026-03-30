using Confluent.Kafka;
using Confluent.SchemaRegistry;
using CustomerManagementApi.Application.Commons;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Infrastructure.Kafka;

/// <summary>
/// Fornece uma classe estática para configurar dependências relacionadas ao Kafka.
/// Esta classe é responsável por configurar o produtor Kafka e o registro de esquema, registrando-os no contêiner de injeção de dependência.
/// </summary>
[ExcludeFromCodeCoverage]
public static class KafkaSetup
{
    /// <summary>
    /// Configura e registra as dependências relacionadas ao produtor Kafka no contêiner de injeção de dependência.
    /// </summary>
    public static void ConfigureKafkaProducer(this IServiceCollection services)
    {
        //var producerConfig = new ProducerConfig { BootstrapServers = CommonsConstants.Kafka.BootstrapServer, };

        //if (CommonsConstants.Kafka.UseSaslSsl)
        //{
        //    producerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
        //    producerConfig.SaslMechanism = SaslMechanism.Plain;
        //    producerConfig.SaslUsername = CommonsConstants.Kafka.SaslUsername;
        //    producerConfig.SaslPassword = CommonsConstants.Kafka.SaslPassword;
        //    producerConfig.SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https;
        //}

        //services.AddSingleton(producerConfig);

        //services.AddSingleton<ISchemaRegistryClient>(new CachedSchemaRegistryClient(new SchemaRegistryConfig 
        //{ 
        //    Url = CommonsConstants.SchemaRegistry.Url,
        //    BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
        //    BasicAuthUserInfo = $"{CommonsConstants.SchemaRegistry.User}:{CommonsConstants.SchemaRegistry.Password}"
        //}));

        //services.AddSingleton(typeof(IKafkaProducer<>), typeof(KafkaProducer<>));
    }
}
