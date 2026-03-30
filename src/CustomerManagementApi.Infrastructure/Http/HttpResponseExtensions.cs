using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace CustomerManagementApi.Infrastructure.Http;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public static class HttpResponseExtensions
{
    /// <summary>
    /// </summary>
    public static async Task<T?> ContentAsType<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
            
        if (response.StatusCode == HttpStatusCode.NoContent || string.IsNullOrWhiteSpace(content))
            return default;

        return JsonSerializer.Deserialize<T>(content, GetJsonOptionsSerializer());
    }

    public static JsonSerializerOptions GetJsonOptionsSerializer() => new()
    {
        PropertyNameCaseInsensitive = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };

    /// <summary>
    /// Detalha o erro retornado pelo servidor antes de lançar uma exceção.
    /// </summary>
    public static async Task EnsureSuccessStatusCodeDetailed(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var requestUrl = response.RequestMessage?.RequestUri?.ToString() ?? string.Empty;
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var message = $"Falha ao chamar '{requestUrl}' (StatusCode: {response.StatusCode})";

        if (!string.IsNullOrWhiteSpace(content))
            message += $" Conteúdo retornado: {content}";

        throw new HttpRequestException(message, null, response.StatusCode);
    }
}
