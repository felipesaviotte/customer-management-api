using CustomerManagementApi.Application.Commons;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CustomerManagementApi.Api.Configurations;

/// <summary>
/// Filtro de operação Swagger que adiciona o header <c>appOrigin</c> como parâmetro obrigatório
/// em todos os endpoints, exceto no webhook de atualização de status (<c>POST /links/status</c>).
/// </summary>
[ExcludeFromCodeCoverage]
public class AppOriginHeaderOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethod = context.ApiDescription.HttpMethod;
        var relativePath = context.ApiDescription.RelativePath ?? string.Empty;

        // O webhook POST /links/status é chamado pelo PaymentCore e não requer appOrigin
        if (string.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase)
            && relativePath.TrimStart('/').Equals("links/status", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        operation.Parameters ??= new List<OpenApiParameter>();

        // Evita duplicação: [FromHeader] no HeaderCommon já gera o parâmetro automaticamente
        var alreadyExists = operation.Parameters.Any(p =>
            string.Equals(p.Name, CommonsConstants.ORIGIN_HEADER, StringComparison.OrdinalIgnoreCase));

        if (!alreadyExists)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = CommonsConstants.ORIGIN_HEADER,
                In = ParameterLocation.Header,
                Required = true,
                Description = "Identificador do sistema de origem da requisição (ex: \"SistemaX\", \"Backoffice\").",
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}
