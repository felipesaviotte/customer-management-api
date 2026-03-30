using CustomerManagementApi.Application.Commons;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CustomerManagementApi.Api.HealthChecks;

/// <summary>
/// Classe de verificação de saúde para o MongoDB, implementando a interface IHealthCheck. Esta classe é responsável por verificar a conectividade e a saúde do banco de dados MongoDB utilizado pela aplicação.
/// </summary>
/// <param name="mongoClient"></param>
[ExcludeFromCodeCoverage]
public class MongoDbHealthCheck(IMongoClient mongoClient) : IHealthCheck
{
    private readonly IMongoClient _mongoClient = mongoClient;

    /// <summary>
    /// Health check para verificar a conectividade com o MongoDB. Ele tenta executar um comando "ping" no banco de dados para garantir que a conexão está funcionando corretamente. Se o comando for bem-sucedido, retorna um resultado saudável; caso contrário, retorna um resultado não saudável com a mensagem de erro.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            var database = _mongoClient.GetDatabase(CommonsConstants.Mongo.Database);
            await database.RunCommandAsync(
                new BsonDocumentCommand<BsonDocument>(new BsonDocument("ping", 1)),
                cancellationToken: cts.Token);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}
