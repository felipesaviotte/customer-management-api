using CustomerManagementApi.Application.Mapper;
using CustomerManagementApi.Application.Ports.Inbound;
using CustomerManagementApi.Application.Ports.Outbound;
using CustomerManagementApi.Application.RequestModel;
using CustomerManagementApi.Application.ResponseModel;
using CustomerManagementApi.Application.ValueObjects;
using CustomerManagementApi.Domain.Entities;
using CustomerManagementApi.Domain.ValueObjects;
using Flunt.Notifications;
using Flunt.Validations;

namespace CustomerManagementApi.Application.UseCases;

/// <summary>
/// Caso de uso responsável por validar e salvar um novo cliente.
/// </summary>
public class SaveCustomerUseCase(ICustomerRepository customerRepository) : ISaveCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository = customerRepository;

    /// <summary>
    /// Valida os dados do cliente e persiste no repositório.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="customerRequestModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public async Task<CustomerResponseModel> Executar(string customerId, CustomerRequestModel customerRequestModel, CancellationToken cancellationToken = default)
    {
        if (customerRequestModel == null)
            throw new ArgumentNullException(nameof(customerRequestModel), "O objeto de solicitação não pode ser nulo.");

        var notifications = Validate(customerRequestModel);
        if (notifications.Count != 0)
            throw new ValidationException(notifications);

        Customer entity;
        if (!string.IsNullOrWhiteSpace(customerId))
        {
            _ = await _customerRepository.GetById(customerId, cancellationToken)
                ?? throw new KeyNotFoundException($"Cliente com ID '{customerId}' não encontrado.");

            entity = CustomerMapper.ToEntity(customerId, customerRequestModel);
            await _customerRepository.Update(entity, cancellationToken);
        }
        else
        {
            customerId = Guid.NewGuid().ToString();
            entity = CustomerMapper.ToEntity(customerId, customerRequestModel);
            await _customerRepository.Insert(entity, cancellationToken);
        }

        //TODO: Implementar a produção do evento de Customer no Kafka
        //Implementação ficaria na InfraEstrutura, mas a chamada ficaria aqui, após a persistência do cliente.
        //ICustomerProducer.Produce();

        return CustomerMapper.ToResponse(entity);
    }

    private static List<Notification> Validate(CustomerRequestModel request)
    {
        Document.Create(request.DocumentNumber, request.DocumentType);
        Email.Create(request.Email);
        if (!string.IsNullOrWhiteSpace(request.Phone))
            Phone.Create(request.Phone);

        var contract = new Contract<Notification>()
            .IsNotNullOrWhiteSpace(request.Name, nameof(request.Name), "O nome do cliente é obrigatório.")
            .IsTrue(request.Name.Length <= 150, nameof(request.Name), "O nome deve ter no máximo 150 caracteres.")
            .IsNotNullOrWhiteSpace(request.DocumentNumber, nameof(request.DocumentNumber), "O número do documento é obrigatório.");

        return [.. contract.Notifications];
    }
}
