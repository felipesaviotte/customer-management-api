using CustomerManagementApi.Domain.Entities;

namespace CustomerManagementApi.Application.Ports.Outbound
{
    /// <summary>
    /// Classe de contrato para o repositório de clientes, define as operações de acesso a dados relacionadas a clientes.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Busca um cliente pelo ID do link do pagamento
        /// </summary>
        public Task<Customer?> GetById(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Insere um cliente no banco de dados
        /// </summary>
        public Task Insert(Customer customer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Atualiza um cliente no banco de dados
        /// </summary>
        public Task Update(Customer customer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove um cliente do banco de dados pelo seu identificador.
        /// </summary>
        public Task Delete(string id, CancellationToken cancellationToken = default);
    }
}
