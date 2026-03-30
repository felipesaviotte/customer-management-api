using CustomerManagementApi.Application.Ports.Inbound;
using CustomerManagementApi.Application.Queries;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Application;

/// <summary>
/// Classe responsável por configurar os serviços da aplicação.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Setup
{
    /// <summary>
    /// Adiciona os serviços da aplicação ao contêiner de injeção de dependência.
    /// </summary>
    public static IServiceCollection AddApplications(this IServiceCollection services)
    {
        services.AddScoped<ICustomerQueryService, CustomerQueryService>();
        services.AddScoped<ISaveCustomerUseCase, UseCases.SaveCustomerUseCase>();       

        return services;
    }
}
