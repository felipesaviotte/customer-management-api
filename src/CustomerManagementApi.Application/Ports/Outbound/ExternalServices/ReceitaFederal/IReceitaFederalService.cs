using CustomerManagementApi.Application.Ports.Outbound.ExternalServices.ReceitaFederal.Responses;
using CustomerManagementApi.Domain.ValueObjects;

namespace CustomerManagementApi.Application.Ports.Outbound.ExternalServices.AdmContratos;

/// <summary>
/// interface para o serviço de consulta à Receita Federal, responsável por obter informações de um CNPJ junto à Receita Federal.
/// </summary>
public interface IReceitaFederalService
{
    /// <summary>
    /// Obtém informações de um CNPJ junto à Receita Federal. Retorna null se o CNPJ não for encontrado ou se ocorrer um erro na consulta.
    /// </summary>
    /// <param name="cnpj"></param>
    /// <returns></returns>
    public Task<ReceitaFederalResponses?> GetCnpj(string cnpj);
}
