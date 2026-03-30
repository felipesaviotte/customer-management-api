using CustomerManagementApi.Application.Commons;
using CustomerManagementApi.Application.Logs;
using CustomerManagementApi.Application.Ports.Outbound.ExternalServices.AdmContratos;
using CustomerManagementApi.Application.Ports.Outbound.ExternalServices.ReceitaFederal.Responses;
using Polly;
using Polly.Retry;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Infrastructure.ExternalServices;

/// <summary>
/// Classe responsável por realizar chamadas ao serviço da Receita Federal.
/// </summary>
[ExcludeFromCodeCoverage]
public class ReceitaFederalService : IReceitaFederalService
{
    private readonly ILoggingService _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly AsyncRetryPolicy _retryPolicy;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="clientFactory"></param>
    public ReceitaFederalService(ILoggingService logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;

        _retryPolicy = Policy
                .Handle<Exception>()
                .RetryAsync(3, (exception, retryCount) =>
                {
                    _logger.Error($"Tentativa {retryCount} falhou. Exception: {exception.Message}.", exception);
                });
    }

    /// <summary>
    /// Obtém informações de um CNPJ junto à Receita Federal. Retorna null se o CNPJ não for encontrado ou se ocorrer um erro na consulta.
    /// </summary>
    /// <param name="cnpj"></param>
    /// <returns></returns>
    public async Task<ReceitaFederalResponses?> GetCnpj(string cnpj)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var client = _clientFactory.CreateClient(nameof(ReceitaFederalService));
                client.DefaultRequestHeaders.Add("x-api-key", CommonsConstants.ReceitaFederal.ApiKey);

                //return await client.GetJsonAsync<ContractSegmentResponses?>($"api/{cnpj.ToString()}");

                //Mock para exempiificar o funcionamento
                return await Task.FromResult(new ReceitaFederalResponses() { Cnpj = cnpj, FullName = "Teste", Situacao = "Ativo" });
            });
        }
        catch (Exception ex)
        {
            _logger.Error($"Erro ao obter dados da API da Refeita Federal {cnpj}: {ex.Message}", ex);
            throw;
        }
    }
}
