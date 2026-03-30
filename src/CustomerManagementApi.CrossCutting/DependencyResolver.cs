using CustomerManagementApi.Application;
using CustomerManagementApi.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.CrossCutting;

/// <summary>
/// Fornece uma classe estática para resolver dependências na aplicação.
/// Esta classe é responsável por registrar serviços de diferentes camadas
/// (Infraestrutura, Aplicação e Domínio) no contêiner de injeção de dependência.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyResolver
{
    /// <summary>
    /// Registra todas as dependências necessárias para a aplicação no contêiner fornecido.
    /// </summary>
    public static void AddDependencies(this IServiceCollection services)
    {
        services.AddInfrastructure();
        services.AddApplications();
    }
}
