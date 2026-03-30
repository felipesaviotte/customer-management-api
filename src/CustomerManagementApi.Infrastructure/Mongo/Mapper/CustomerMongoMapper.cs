using CustomerManagementApi.Application.ResponseModel;
using CustomerManagementApi.Domain.Entities;
using CustomerManagementApi.Domain.Enums;
using CustomerManagementApi.Infrastructure.Mongo.Document;

namespace CustomerManagementApi.Infrastructure.Mongo.Mapper;

/// <summary>
/// Classe de mapeamento responsável por converter entre as entidades do domínio e os documentos Mongo relacionados a clientes.
/// </summary>
public static partial class CustomerMongoMapper
{
    /// <summary>
    /// Converte uma entidade Customer para um documento CustomerMongoDocument, mapeando os campos relevantes para a persistência no MongoDB.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static CustomerMongoDocument ToDocument(Customer request)
    {
        return new CustomerMongoDocument
        {
            Id = request.Id,
            Name = request.Name,
            DocumentType = (int)request.DocumentType,
            DocumentNumber = request.DocumentNumber.Value,
            Email = request.Email.Value,
            Phone = request.Phone == null ? null : request.Phone.Value,
            Status = (int)request.Status,
            CreatedDate = request.CreatedDate
        };
    }

    /// <summary>
    /// Converte um documento CustomerMongoDocument para uma entidade Customer, mapeando os campos relevantes para a lógica de negócios do domínio.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static Customer ToEntity(CustomerMongoDocument request)
    {
        return new Customer
        {
            Id = request.Id,
            Name = request.Name,
            DocumentType = (DocumentType)request.DocumentType,
            DocumentNumber = Domain.ValueObjects.Document.Create(request.DocumentNumber, (DocumentType)request.DocumentType),
            Email = Domain.ValueObjects.Email.Create(request.Email),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : Domain.ValueObjects.Phone.Create(request.Phone),
            Status = (CustomerStatus)request.Status,
            CreatedDate = request.CreatedDate
        };
    }

    public static CustomerResponseModel ToModel(CustomerMongoDocument request)
    {
        return new CustomerResponseModel
        {
            Id = request.Id,
            Name = request.Name,
            DocumentType = (DocumentType)request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            Email = request.Email,
            Phone = request.Phone,
            Status = (CustomerStatus)request.Status
        };
    }
}
