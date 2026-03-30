using CustomerManagementApi.Application.RequestModel;
using CustomerManagementApi.Application.ResponseModel;

namespace CustomerManagementApi.Application.Ports.Inbound
{
    /// <summary>
    /// interface para o caso de uso de salvar cliente, define o contrato para a implementação do caso de uso que será responsável por processar a lógica de negócio relacionada à criação de um novo cliente e retornar a resposta adequada. Essa interface é parte da camada de aplicação e é utilizada para garantir a separação de responsabilidades e facilitar a manutenção do código.
    /// </summary>
    public interface ISaveCustomerUseCase
    {
        /// <summary>
        /// Salvar dados do cliente
        /// </summary>
        /// <param name="customerId"
        /// <param name="customerRequestModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CustomerResponseModel> Executar(string customerId, CustomerRequestModel customerRequestModel, CancellationToken cancellationToken = default);
    }
}
