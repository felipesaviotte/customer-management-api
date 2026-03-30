using CustomerManagementApi.Application.Commons;
using CustomerManagementApi.Application.Ports.Outbound;
using CustomerManagementApi.Application.ResponseModel;
using CustomerManagementApi.Domain.Entities;
using CustomerManagementApi.Domain.Enums;
using CustomerManagementApi.Domain.Repositories;
using CustomerManagementApi.Infrastructure.Mongo.Document;
using CustomerManagementApi.Infrastructure.Mongo.Mapper;
using CustomerManagementApi.Infrastructure.Mongo.Repositories;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
using static CustomerManagementApi.Application.Commons.CommonsConstants;

namespace CustomerManagementApi.Infrastructure.Db.Repositories;

/// <summary>
/// Repositório responsável por lidar com operações de clientes
/// </summary>
[ExcludeFromCodeCoverage]
public class CustomerRepository(IContextMongo context) : BaseMongoRepository<CustomerMongoDocument>(context, CommonsConstants.Mongo.CustomerCollection), ICustomerQueryRepository, ICustomerRepository
{
    /// <summary>
    /// Obtém uma lista de clientes paginada.
    /// </summary>
    public async Task<GenericResponseModel<CustomerResponseModel>> GetCustomers(int page = PaginationDefaults.DefaultPage, int pageSize = PaginationDefaults.DefaultPageSize, string? name = null, CustomerStatus? status = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CustomerMongoDocument>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(name))
            filter &= Builders<CustomerMongoDocument>.Filter.Regex(f => f.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));

        if (status.HasValue)
            filter &= Builders<CustomerMongoDocument>.Filter.Eq(f => f.Status, (int)status.Value);

        if (pageSize > PaginationDefaults.MaxPageSize)
            pageSize = PaginationDefaults.MaxPageSize;

            var documents = await DbSet.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        var data = Enumerable.Empty<CustomerResponseModel>();

        if (documents != null && documents.Any())
            data = documents.Select(CustomerMongoMapper.ToModel).ToList();
        
        return new GenericResponseModel<CustomerResponseModel>(page, pageSize, data);
    }

    /// <summary>
    /// Obter um cliente pelo ID
    /// </summary>
    public async Task<CustomerResponseModel?> GetCustomersById(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CustomerMongoDocument>.Filter.Eq(field => field.Id, id);
        var result = await Get(filter, cancellationToken);
        if (result == null)
            return null;

        return CustomerMongoMapper.ToModel(result);
    }

    /// <summary>
    /// Busca um cliente pelo ID do link do pagamento
    /// </summary>
    /// <returns></returns>
    public async Task<Customer?> GetById(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CustomerMongoDocument>.Filter.Eq(field => field.Id, id);
        var result = await Get(filter, cancellationToken);
        if (result == null)
            return null;

        return CustomerMongoMapper.ToEntity(result);
    }

    /// <summary>
    /// Insere um cliente no banco de dados
    /// </summary>
    public async Task Insert(Customer customer, CancellationToken cancellationToken = default)
    {
        var document = CustomerMongoMapper.ToDocument(customer);
        await Insert(document, cancellationToken);
    }

    /// <summary>
    /// Atualiza um cliente no banco de dados
    /// </summary>
    public async Task Update(Customer customer, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CustomerMongoDocument>.Filter.Eq(field => field.Id, customer.Id);
        var update = Builders<CustomerMongoDocument>.Update
            .Set(f => f.Name, customer.Name)
            .Set(f => f.DocumentType, (int)customer.DocumentType)
            .Set(f => f.DocumentNumber, customer.DocumentNumber.Value)
            .Set(f => f.Email, customer.Email.Value)
            .Set(f => f.Phone, customer.Phone?.Value)
            .Set(f => f.Status, (int)customer.Status);

        await Update(filter, update, cancellationToken);
    }

    /// <summary>
    /// Obtém a contagem total de clientes
    /// </summary>
    public async Task<long> Count(CustomerStatus? status = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CustomerMongoDocument>.Filter.Empty;

        if (status.HasValue)
            filter &= Builders<CustomerMongoDocument>.Filter.Eq(f => f.Status, (int)status.Value);

        return await DbSet.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Remove um cliente do banco de dados pelo seu identificador.
    /// </summary>
    public async Task Delete(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CustomerMongoDocument>.Filter.Eq(field => field.Id, id);
        await DbSet.DeleteOneAsync(filter, cancellationToken);
    }
}
