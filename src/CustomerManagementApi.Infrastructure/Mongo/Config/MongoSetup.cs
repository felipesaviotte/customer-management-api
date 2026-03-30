using CustomerManagementApi.Application.Commons;
using CustomerManagementApi.Domain.Entities;
using CustomerManagementApi.Domain.Repositories;
using CustomerManagementApi.Infrastructure.Mongo.Document;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;

namespace CustomerManagementApi.Infrastructure.Mongo.Config;

/// <summary>
/// Classe estática responsável por configurar a conexão com o MongoDB e registrar os serviços relacionados no contêiner de injeção de dependência. Ela define as configurações de conexão, cria o cliente MongoDB, configura os índices necessários para otimizar as consultas e registra os serviços de contexto Mongo e cliente MongoDB para serem usados em toda a aplicação.
/// </summary>
[ExcludeFromCodeCoverage]
public static class MongoSetup
{
    /// <summary>
    /// Adiciona a configuração do MongoDB ao contêiner de injeção de dependência. Configura as opções de conexão, incluindo SSL e tempo limite de fila de espera. Cria um cliente MongoDB e configura os índices necessários para otimizar as consultas. Registra o cliente MongoDB como um serviço singleton e o contexto Mongo como um serviço com escopo, garantindo que as dependências sejam resolvidas corretamente em toda a aplicação.
    /// </summary>
    /// <param name="services"></param>
    public static void AddMongo(this IServiceCollection services)
    {
        var settings = MongoClientSettings.FromConnectionString(CommonsConstants.Mongo.ConnectionString);
        if (!CommonsConstants.Mongo.Local)
        {
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        }
        settings.WaitQueueTimeout = TimeSpan.FromSeconds(15);

        var mongoClient = new MongoClient(settings);
        mongoClient.ConfigureIndexes().Wait();

        services.AddSingleton<IMongoClient>(c => mongoClient);

        services.AddScoped<IContextMongo, ContextMongo>();
    }

    /// <summary>
    /// Cria índices para a coleção de clientes, garantindo que as consultas sejam otimizadas. Esses índices permitem que o MongoDB localize rapidamente os documentos com base nesses campos, melhorando o desempenho das operações de leitura. A criação de índices é uma prática recomendada para coleções que serão consultadas com frequência usando esses campos como critérios de pesquisa.
    /// </summary>
    /// <param name="mongoClient"></param>
    /// <returns></returns>
    private static async Task ConfigureIndexes(this MongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(CommonsConstants.Mongo.Database);

        var customerCollection = database.GetCollection<CustomerMongoDocument>(CommonsConstants.Mongo.CustomerCollection);
        await customerCollection.Indexes.CreateOneAsync(new CreateIndexModel<CustomerMongoDocument>(Builders<CustomerMongoDocument>.IndexKeys.Ascending(m => m.Id)));
        await customerCollection.Indexes.CreateOneAsync(new CreateIndexModel<CustomerMongoDocument>(Builders<CustomerMongoDocument>.IndexKeys.Ascending(m => m.Name)));
        await customerCollection.Indexes.CreateOneAsync(new CreateIndexModel<CustomerMongoDocument>(Builders<CustomerMongoDocument>.IndexKeys.Ascending(m => m.DocumentNumber)));

        //Exemplo de criação de índice TTL para a coleção de clientes, onde os documentos serão automaticamente removidos após 30 dias com base no campo CreatedDate.
        //await CreateTtlIndex<Customer>(database, CommonsConstants.Mongo.CustomerCollection, "customer_ttl_index", c => c.CreatedDate, TimeSpan.FromDays(30));
    }

    /// <summary>
    /// Cria um índice TTL (Time To Live) para a coleção especificada, garantindo que os documentos sejam automaticamente removidos após um período definido. O índice é criado apenas se ainda não existir, evitando duplicações. O campo especificado é usado para determinar a idade dos documentos, e o MongoDB cuidará da remoção automática dos documentos expirados com base nesse campo.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="database"></param>
    /// <param name="collectionName"></param>
    /// <param name="indexName"></param>
    /// <param name="field"></param>
    /// <param name="expireAfter"></param>
    /// <returns></returns>
    private static async Task CreateTtlIndex<T>(IMongoDatabase database, string collectionName, string indexName, System.Linq.Expressions.Expression<Func<T, object>> field, TimeSpan expireAfter)
    {
        var collection = database.GetCollection<T>(collectionName);
        var indexes = await collection.Indexes.ListAsync();
        var indexList = await indexes.ToListAsync();
        var ttlExists = indexList.Any(i => i["name"].AsString == indexName);
        if (!ttlExists)
        {
            var indexModel = new CreateIndexModel<T>(
                Builders<T>.IndexKeys.Ascending(field),
                new CreateIndexOptions { Name = indexName, ExpireAfter = expireAfter });
            await collection.Indexes.CreateOneAsync(indexModel);
        }
    }
}