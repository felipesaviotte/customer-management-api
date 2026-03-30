using CustomerManagementApi.Application.RequestModel;
using CustomerManagementApi.Application.ResponseModel;
using CustomerManagementApi.Domain.Entities;
using CustomerManagementApi.Domain.ValueObjects;

namespace CustomerManagementApi.Application.Mapper;

/// <summary>
/// Classe de mapeamento responsável por converter entre os modelos de solicitação/resposta e as entidades do domínio relacionadas a clientes.
/// </summary>
public static partial class CustomerMapper
{
    /// <summary>
    /// Converte um objeto CreateCustomerRequestModel para uma entidade Customer, preenchendo os campos necessários e gerando um novo ID para o cliente.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static Customer ToEntity(string customerId, CustomerRequestModel request)
    {
        return new Customer
        {
            Id = customerId,
            Name = request.Name,
            DocumentType = request.DocumentType,
            DocumentNumber = Document.Create(request.DocumentNumber, request.DocumentType),
            Email = Email.Create(request.Email),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : Phone.Create(request.Phone),
            Status = request.Status,
            CreatedDate = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Converte uma entidade Customer para um objeto CreateCustomerResponseModel, mapeando os campos relevantes para a resposta da API.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static CustomerResponseModel ToResponse(Customer entity)
    {
        return new CustomerResponseModel
        {
            Id = entity.Id,
            Name = entity.Name,
            DocumentType = entity.DocumentType,
            DocumentNumber = entity.DocumentNumber.Value,
            Email = entity.Email.Value,
            Phone = entity.Phone?.Value,
            Status = entity.Status
        };
    }
}
