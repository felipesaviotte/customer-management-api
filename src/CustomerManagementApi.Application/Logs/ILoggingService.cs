using System.Runtime.CompilerServices;

namespace CustomerManagementApi.Application.Logs;

/// <summary>
/// Interface responsável por definir métodos para registro de logs na aplicação.
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Registra uma mensagem informativa
    /// </summary>
    void Information(string context, string? message = null, KeyValuePair<string, object>[]? customParameters = null, [CallerMemberName] string method = "");

    /// <summary>
    /// Registra uma mensagem de aviso.
    /// </summary>
    void Warning(string context, string? message = null, KeyValuePair<string, object>[]? customParameters = null, [CallerMemberName] string method = "");

    /// <summary>
    /// Registra uma mensagem de erro.
    /// </summary>
    void Error(string context, Exception exception, string? message = null, KeyValuePair<string, object>[]? customParameters = null, [CallerMemberName] string method = "");
}
