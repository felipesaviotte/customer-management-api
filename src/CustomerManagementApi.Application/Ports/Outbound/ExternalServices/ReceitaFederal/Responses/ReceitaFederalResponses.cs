using System.Text.Json.Serialization;

namespace CustomerManagementApi.Application.Ports.Outbound.ExternalServices.ReceitaFederal.Responses;

/// <summary>
/// Classe de resposta para os dados do CNPJ obtidos da Receita Federal.
/// </summary>
public class ReceitaFederalResponses
{
    /// <summary>
    /// Cnpj
    /// </summary>
    [JsonPropertyName("cnpj")]
    public string? Cnpj { get; set; }

    /// <summary>
    /// Razao Social
    /// </summary>
    [JsonPropertyName("razaoSocial")]
    public string? FullName { get; set; }

    /// <summary>
    /// Sigla do Cnpj
    /// </summary>
    [JsonPropertyName("situacao")]
    public string? Situacao { get; set; }
}