using MongoDB.Driver;

namespace CustomerManagementApi.Domain.Repositories;

/// <summary>
/// Representa a interface base para operações de repositório no MongoDB.
/// </summary>
public interface IBaseMongoRepository<TEntity> : IDisposable where TEntity : class
{
    /// <summary>
    /// Insere uma nova entidade na coleção do MongoDB.
    /// </summary>
    Task Insert(TEntity obj, CancellationToken cancellationToken);

    /// <summary>
    /// Recupera uma lista de entidades da coleção do MongoDB com base no filtro especificado.
    /// </summary>
    Task<IEnumerable<TEntity>> List(FilterDefinition<TEntity> filter, CancellationToken cancellationToken);

    /// <summary>
    /// Recupera uma única entidade da coleção do MongoDB com base no filtro especificado.
    /// </summary>
    Task<TEntity> Get(FilterDefinition<TEntity> filter, CancellationToken cancellationToken);
}