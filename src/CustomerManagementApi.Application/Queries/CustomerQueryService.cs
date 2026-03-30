using CustomerManagementApi.Application.Ports.Inbound;
using CustomerManagementApi.Application.Ports.Outbound;
using CustomerManagementApi.Application.ResponseModel;
using static CustomerManagementApi.Application.Commons.CommonsConstants;

namespace CustomerManagementApi.Application.Queries;

/// <summary>
/// Serviço responsável por criar links de pagamento
/// </summary>
public class CustomerQueryService(ICustomerQueryRepository customerQueryRepository) : ICustomerQueryService
{
    private readonly ICustomerQueryRepository _customerQueryRepository = customerQueryRepository;

    /// <summary>
    /// Obtém uma lista de clientes paginada.
    /// </summary>
    /// <param name="page">Número da página a ser retornada (padrão é PaginationDefaults.DefaultPage).</param>
    /// <param name="pageSize">Número de clientes por página (padrão é PaginationDefaults.DefaultPageSize).</param>
    /// <param name="name">Nome do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento para operações assíncronas.</param>
    /// <returns>Um objeto GenericResponseModel contendo a lista de clientes e informações adicionais.</returns>
    public async Task<GenericResponseModel<CustomerResponseModel>> GetCustomers(int page = PaginationDefaults.DefaultPage, int pageSize = PaginationDefaults.DefaultPageSize, string? name = null, CancellationToken cancellationToken = default)
    {
        return await _customerQueryRepository.GetCustomers(page, pageSize, name, cancellationToken);
    }

    /// <summary>
    /// Obtém um cliente pelo seu ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CustomerResponseModel> GetCustomersById(string id, CancellationToken cancellationToken = default)
    {
        return await _customerQueryRepository.GetCustomersById(id, cancellationToken) 
            ?? throw new KeyNotFoundException($"Cliente com ID '{id}' não encontrado.");
    }

    /// <summary>
    /// Obtém a contagem total de clientes.
    /// </summary>
    public async Task<long> Count(CancellationToken cancellationToken = default)
    {
        return await _customerQueryRepository.Count(cancellationToken);
    }
}
