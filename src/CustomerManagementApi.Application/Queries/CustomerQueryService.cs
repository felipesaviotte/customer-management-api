using CustomerManagementApi.Application.Ports.Inbound;
using CustomerManagementApi.Application.Ports.Outbound;
using CustomerManagementApi.Application.ResponseModel;
using CustomerManagementApi.Domain.Enums;
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
    public async Task<GenericResponseModel<CustomerResponseModel>> GetCustomers(int page = PaginationDefaults.DefaultPage, int pageSize = PaginationDefaults.DefaultPageSize, string? name = null, CustomerStatus? status = null, CancellationToken cancellationToken = default)
    {
        return await _customerQueryRepository.GetCustomers(page, pageSize, name, status, cancellationToken);
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
    public async Task<long> Count(CustomerStatus? status = null, CancellationToken cancellationToken = default)
    {
        return await _customerQueryRepository.Count(status, cancellationToken);
    }
}
