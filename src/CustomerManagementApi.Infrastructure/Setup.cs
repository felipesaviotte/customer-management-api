using CustomerManagementApi.Application.Commons;
using CustomerManagementApi.Application.Ports.Outbound;
using CustomerManagementApi.Application.Ports.Outbound.ExternalServices.AdmContratos;
using CustomerManagementApi.Infrastructure.Db.Repositories;
using CustomerManagementApi.Infrastructure.ExternalServices;
using CustomerManagementApi.Infrastructure.Kafka;
using CustomerManagementApi.Infrastructure.Mongo.Config;
using CustomerManagementApi.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Infrastructure;

/// <summary>
/// Fornece uma classe estática para configurar e registrar dependências no nível de infraestrutura.
/// Esta classe é responsável por configurar serviços como repositórios, serviços externos e infraestrutura de mensageria no contêiner de injeção de dependência.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Setup
{
    /// <summary>
    /// Registra todas as dependências no nível de infraestrutura no contêiner de injeção de dependência fornecido.
    /// </summary>
    public static void AddInfrastructure(this IServiceCollection services)
    {
        if (CommonsConstants.UseInMemoryDb)
        {
            services.AddSingleton<ICustomerQueryRepository, InMemoryCustomerRepository>();
            services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
        }
        else
        {
            services.AddMongo();
            services.AddScoped<ICustomerQueryRepository, CustomerRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }

        services.ConfigureKafkaProducer();

        services.AddHttpClient(nameof(ReceitaFederalService), config => config.BaseAddress = new Uri(CommonsConstants.ReceitaFederal.BaseUrl));

        services.AddScoped<IReceitaFederalService, ReceitaFederalService>();
    }
}