using MongoDB.Driver;
using System;

namespace CustomerManagementApi.Domain.Repositories;

/// <summary>
/// Representa a interface do contexto do MongoDB para gerenciar coleções do banco de dados.
/// </summary>
public interface IContextMongo : IDisposable
{
    /// <summary>
    /// Recupera uma coleção do MongoDB pelo seu nome.
    /// </summary>
    IMongoCollection<T> GetCollection<T>(string name);
}