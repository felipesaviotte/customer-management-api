using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Runtime.CompilerServices;
using CustomerManagementApi.Application.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Application.Logs;

/// <summary>
/// Classe responsável por realizar o registro de logs utilizando a biblioteca Serilog.
/// </summary>
[ExcludeFromCodeCoverage]
public class SerilogLoggingService : ILoggingService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    /// <summary>
    /// Registra uma mensagem informativa.
    /// </summary>
    public void Information(string context, string? message = null, KeyValuePair<string, object>[]? customParameters = null, [CallerMemberName] string method = "")
    {
        (StringBuilder messageTemplateBuilder, object[] parameters) = BuildSerilogModel(message, context, customParameters, method);
        Log.Information(messageTemplateBuilder.ToString(), parameters);
    }

    /// <summary>
    /// Registra uma mensagem de aviso.
    /// </summary>
    public void Warning(string context, string? message = null, KeyValuePair<string, object>[]? customParameters = null, [CallerMemberName] string method = "")
    {
        (StringBuilder messageTemplateBuilder, object[] parameters) = BuildSerilogModel(message, context, customParameters, method);
        Log.Warning(messageTemplateBuilder.ToString(), parameters);
    }

    /// <summary>
    /// Registra uma mensagem de erro.
    /// </summary>
    public void Error(string context, Exception exception, string? message = null, KeyValuePair<string, object>[]? customParameters = null, [CallerMemberName] string method = "")
    {
        (StringBuilder messageTemplateBuilder, object[] parameters) = BuildSerilogModel(message, context, customParameters, method);

        if (exception != null)
            Log.Error(exception, messageTemplateBuilder.ToString(), parameters);
        else
            Log.Error(messageTemplateBuilder.ToString(), parameters);
    }

    private static (StringBuilder messageTemplateBuilder, object[] parameters) BuildSerilogModel(
        string? message,
        string context,
        KeyValuePair<string, object>[]? customParameters,
        string? method)
    {
        var messageTemplateBuilder = new StringBuilder($"{message} [ context: {{context}} ");
        var parameters = new List<object?>();

        if (!string.IsNullOrEmpty(method))
        {
            messageTemplateBuilder.Append("- method: {method} ");
            parameters.Add(method);
        }

        customParameters ??= [];

        parameters =
        [
            .. parameters,
                .. customParameters.Select(parameter =>
                {
                    if (parameter.Value is null)
                        return null;

                    messageTemplateBuilder.Append($"- {parameter.Key}: {{{parameter.Key}}} ");
                    return parameter.Value is string ? parameter.Value :
                    JsonSerializer.Serialize(parameter.Value, JsonOptions);
                }),
            ];

        parameters.Insert(0, context);

        messageTemplateBuilder.Append(']');

        return (messageTemplateBuilder, parameters.WhereNotNull().ToArray());
    }
}
