using CustomerManagementApi.Application.Ports.Inbound;
using CustomerManagementApi.Application.Ports.Outbound;

namespace CustomerManagementApi.Application.UseCases;

/// <summary>
/// Caso de uso responsável por remover um cliente.
/// </summary>
public class DeleteCustomerUseCase(ICustomerRepository customerRepository) : IDeleteCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository = customerRepository;

    /// <summary>
    /// Valida a existência do cliente e o remove do repositório.
    /// </summary>
    /// <param name="customerId">Identificador do cliente a ser removido.</param>
    /// <param name="cancellationToken">Token de cancelamento para operações assíncronas.</param>
    public async Task Executar(string customerId, CancellationToken cancellationToken = default)
    {
        _ = await _customerRepository.GetById(customerId, cancellationToken)
            ?? throw new KeyNotFoundException($"Cliente com ID '{customerId}' não encontrado.");

        await _customerRepository.Delete(customerId, cancellationToken);

        //TODO: Implementar a produção do evento de Customer no Kafka
        //Implementação ficaria na InfraEstrutura, mas a chamada ficaria aqui, após a exclusão do cliente.
        //ICustomerProducer.Produce();
    }
}
