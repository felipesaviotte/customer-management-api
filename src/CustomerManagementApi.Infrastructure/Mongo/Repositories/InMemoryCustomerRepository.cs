using CustomerManagementApi.Application.Ports.Outbound;
using CustomerManagementApi.Application.ResponseModel;
using CustomerManagementApi.Domain.Entities;
using CustomerManagementApi.Domain.Enums;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using static CustomerManagementApi.Application.Commons.CommonsConstants;

namespace CustomerManagementApi.Infrastructure.Mongo.Repositories;

/// <summary>
/// Implementação in-memory dos repositórios de cliente, simulando o MongoDB com um dicionário estático.
/// Ativada pela variável de ambiente USE_INMEMORY_DB=true.
/// </summary>
[ExcludeFromCodeCoverage]
public class InMemoryCustomerRepository : ICustomerQueryRepository, ICustomerRepository
{
    private static readonly ConcurrentDictionary<string, Customer> _store = new();

    /// <inheritdoc/>
    public Task<GenericResponseModel<CustomerResponseModel>> GetCustomers(
        int page = PaginationDefaults.DefaultPage,
        int pageSize = PaginationDefaults.DefaultPageSize,
        string? name = null,
        CustomerStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _store.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (pageSize > PaginationDefaults.MaxPageSize)
            pageSize = PaginationDefaults.MaxPageSize;

        var data = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToResponse)
            .ToList();

        return Task.FromResult(new GenericResponseModel<CustomerResponseModel>(page, pageSize, data));
    }

    /// <inheritdoc/>
    public Task<CustomerResponseModel?> GetCustomersById(string id, CancellationToken cancellationToken = default)
    {
        if (_store.TryGetValue(id, out var customer))
            return Task.FromResult<CustomerResponseModel?>(ToResponse(customer));

        return Task.FromResult<CustomerResponseModel?>(null);
    }

    /// <inheritdoc/>
    public Task<long> Count(CustomerStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _store.Values.AsEnumerable();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        return Task.FromResult((long)query.Count());
    }

    /// <inheritdoc/>
    public Task<Customer?> GetById(string id, CancellationToken cancellationToken = default)
    {
        _store.TryGetValue(id, out var customer);
        return Task.FromResult(customer);
    }

    /// <inheritdoc/>
    public Task Insert(Customer customer, CancellationToken cancellationToken = default)
    {
        _store[customer.Id] = customer;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task Update(Customer customer, CancellationToken cancellationToken = default)
    {
        _store[customer.Id] = customer;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task Delete(string id, CancellationToken cancellationToken = default)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    private static CustomerResponseModel ToResponse(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        DocumentType = c.DocumentType,
        DocumentNumber = c.DocumentNumber.Value,
        Email = c.Email.Value,
        Phone = c.Phone?.Value,
        Status = c.Status
    };
}
