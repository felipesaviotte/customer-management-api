using CustomerManagementApi.Application.Commons;
using CustomerManagementApi.Domain.Repositories;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Infrastructure.Mongo.Config;

///<!---->
[ExcludeFromCodeCoverage]
public class ContextMongo(IMongoClient mongoClient) : IContextMongo
{
    private IMongoDatabase? Database { get; set; }

    ///<!---->
    public IClientSessionHandle? Session { get; set; }

    ///<!---->
    public IMongoClient MongoClient { get; set; } = mongoClient;

    ///<!---->
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        ConfigureMongo();

        if (Database == null)
            throw new InvalidOperationException("Database is not configured.");

        return Database.GetCollection<T>(name);
    }

    ///<!---->
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ///<!---->
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Session?.Dispose();
        }
    }

    private void ConfigureMongo()
    {
        Database = MongoClient.GetDatabase(CommonsConstants.Mongo.Database);
        Session = MongoClient.StartSession();
    }
}
