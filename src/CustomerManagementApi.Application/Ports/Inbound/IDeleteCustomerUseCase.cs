namespace CustomerManagementApi.Application.Ports.Inbound
{
    /// <summary>
    /// Interface para o caso de uso de exclusão de cliente.
    /// </summary>
    public interface IDeleteCustomerUseCase
    {
        /// <summary>
        /// Remove um cliente pelo seu identificador.
        /// </summary>
        /// <param name="customerId">Identificador do cliente a ser removido.</param>
        /// <param name="cancellationToken">Token de cancelamento para operações assíncronas.</param>
        public Task Executar(string customerId, CancellationToken cancellationToken = default);
    }
}
