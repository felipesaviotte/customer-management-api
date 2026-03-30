using CustomerManagementApi.Application.ResponseModel;
using static CustomerManagementApi.Application.Commons.CommonsConstants;

namespace CustomerManagementApi.Application.Ports.Outbound
{
    /// <summary>
    /// Classe de interface para o repositório de consulta de clientes.
    /// </summary>
    public interface ICustomerQueryRepository
    {
        /// <summary>
        /// Obtém uma lista de clientes paginada.
        /// </summary>
        /// <param name="page">Número da página a ser retornada (padrão é PaginationDefaults.DefaultPage).</param>
        /// <param name="pageSize">Número de clientes por página (padrão é PaginationDefaults.DefaultPageSize).</param>
        /// <param name="name">Nome do cliente</param>
        /// <param name="cancellationToken">Token de cancelamento para operações assíncronas.</param>
        /// <returns>Um objeto GenericResponseModel contendo a lista de clientes e informações adicionais.</returns>
        public Task<GenericResponseModel<CustomerResponseModel>> GetCustomers(int page = PaginationDefaults.DefaultPage, int pageSize = PaginationDefaults.DefaultPageSize, string? name = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém os detalhes de um cliente específico pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken">Token de cancelamento para operações assíncronas.</param>
        /// <returns></returns>
        public Task<CustomerResponseModel?> GetCustomersById(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém a contagem total de clientes.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento para operações assíncronas.</param>
        /// <returns>Número total de clientes.</returns>
        public Task<long> Count(CancellationToken cancellationToken = default);
    }
}
