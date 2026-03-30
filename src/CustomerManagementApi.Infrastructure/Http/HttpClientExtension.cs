using System.Text.Json;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Infrastructure.Http;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public static class HttpClientExtension
{
    /// <summary>
    /// </summary>
    public static async Task<HttpResponseMessage> PostSerializedAsync(this HttpClient httpClient, string requestUri, object body)
    {
        var content = new StringContent(JsonSerializer.Serialize(body, GetJsonOptionsSerializer()), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(requestUri, content);
        return response;
    }

    private static JsonSerializerOptions GetJsonOptionsSerializer() => new()
    {
        PropertyNameCaseInsensitive = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<T?> GetJsonAsync<T>(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            var jsonResponse = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<T>(jsonResponse, GetJsonOptionsSerializer());
            if (object.Equals(result, default(T)))
                throw new InvalidOperationException("Deserialização retornou nulo.");
            return result;
        }
        throw new InvalidOperationException($"Erro ao obter dados: {response.ReasonPhrase}");
    }
}