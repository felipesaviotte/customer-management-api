using CustomerManagementApi.Domain.Repositories;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Infrastructure.Mongo.Repositories;

/// <summary>
/// Repositório base para gerenciar entidades em um banco de dados MongoDB.
/// Fornece métodos genéricos para operações comuns no banco de dados, como listar, recuperar e inserir entidades.
/// </summary>
/// <typeparam name="TEntity">O tipo da entidade gerenciada pelo repositório.</typeparam>
[ExcludeFromCodeCoverage]
public class BaseMongoRepository<TEntity> : IBaseMongoRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// O contexto do MongoDB usado para interagir com o banco de dados.
    /// </summary>
    protected readonly IContextMongo Context;

    /// <summary>
    /// A coleção do MongoDB para o tipo de entidade especificado.
    /// </summary>
    protected IMongoCollection<TEntity> DbSet;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="BaseMongoRepository{TEntity}"/>.
    /// </summary>
    /// <param name="context">O contexto do MongoDB usado para interagir com o banco de dados.</param>
    /// <param name="collectionName">O nome da coleção do MongoDB para o tipo de entidade.</param>
    protected BaseMongoRepository(IContextMongo context, string collectionName)
    {
        Context = context;
        DbSet = Context.GetCollection<TEntity>(collectionName);
    }

    /// <summary>
    /// Recupera uma lista de entidades do banco de dados que correspondem ao filtro especificado.
    /// </summary>
    /// <param name="filter">A definição do filtro a ser aplicada à consulta.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma coleção de entidades que correspondem ao filtro.</returns>
    public virtual async Task<IEnumerable<TEntity>> List(FilterDefinition<TEntity> filter, CancellationToken cancellationToken)
    {
        var data = await DbSet.FindAsync(filter, cancellationToken: cancellationToken);
        return await data.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Recupera uma única entidade do banco de dados que corresponde ao filtro especificado.
    /// </summary>
    /// <param name="filter">A definição do filtro a ser aplicada à consulta.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>A entidade que corresponde ao filtro, ou null se nenhuma entidade for encontrada.</returns>
    public virtual async Task<TEntity> Get(FilterDefinition<TEntity> filter, CancellationToken cancellationToken)
    {
        var data = await DbSet.FindAsync(filter, cancellationToken: cancellationToken);
        return await data.SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Insere uma nova entidade no banco de dados.
    /// </summary>
    /// <param name="obj">A entidade a ser inserida.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    public virtual async Task Insert(TEntity obj, CancellationToken cancellationToken)
    {
        await DbSet.InsertOneAsync(obj, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Atualiza uma entidade no banco de dados com base no filtro e na definição de atualização especificados.
    /// </summary>
    /// <param name="filter">A definição do filtro para localizar a entidade a ser atualizada.</param>
    /// <param name="update">A definição de atualização especificando as alterações a serem aplicadas.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>True se a atualização foi bem-sucedida; caso contrário, false.</returns>
    public virtual async Task<bool> Update(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update, CancellationToken cancellationToken)
    {
        var result = await DbSet.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount != 0;
    }

    /// <summary>
    /// Libera os recursos usados pelo repositório.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Libera os recursos não gerenciados usados pelo repositório e, opcionalmente, libera os recursos gerenciados.
    /// </summary>
    /// <param name="disposing">True para liberar recursos gerenciados e não gerenciados; false para liberar apenas recursos não gerenciados.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Context?.Dispose();
        }
    }
}
