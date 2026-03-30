using Flunt.Notifications;
using CustomerManagementApi.Application.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CustomerManagementApi.Application.ValueObjects;

/// <summary>
/// Representa o resultado de uma operação.
/// </summary>
[ExcludeFromCodeCoverage]
public class Result
{
    /// <summary>
    /// Verdadeiro se o resultado não contém erros, caso contrário, falso.
    /// </summary>
    [JsonIgnore]
    public bool IsValid => !Notifications.HasAny();

    /// <summary>
    /// Coleção somente leitura de notificações.
    /// </summary>
    public IReadOnlyCollection<Notification> Notifications { get; set; } = [];

    /// <summary>
    /// Construtor com notificações vazias.
    /// </summary>        
    private Result() { }

    /// <summary>
    /// Construtor com notificações.
    /// </summary>
    /// <param name="notifications">Coleção de notificações.</param>
    private Result(IReadOnlyCollection<Notification> notifications) => Notifications = notifications;

    /// <summary>
    /// Retorna um resultado com notificações vazias.
    /// </summary>
    public static Result Ok() => new();

    /// <summary>
    /// Retorna um resultado com notificações.
    /// </summary>
    /// <param name="notifications">Coleção de notificações.</param>
    /// <returns>Resultado com notificações.</returns>
    public static Result Error(IReadOnlyCollection<Notification> notifications) => new(notifications);

    /// <summary>
    /// Retorna um resultado com notificações.
    /// </summary>
    /// <param name="notifications">Array de notificações.</param>
    /// <returns>Resultado com notificações.</returns>
    public static Result Error(params Notification[] notifications) => new(notifications);

    /// <summary>
    /// Retorna um resultado com notificações.
    /// </summary>
    /// <param name="key">Chave da notificação.</param>
    /// <param name="message">Mensagem da notificação (valor padrão: "Parâmetro inválido").</param>
    /// <returns>Resultado com notificações.</returns>
    public static Result Error([Required] string key, string message = "Parâmetro inválido") => new(
    [
        new(key, message)
    ]);
}

/// <summary>
/// Resultado com tipo genérico.
/// </summary>
[ExcludeFromCodeCoverage]
public class Result<T>
{
    /// <summary>
    /// Verdadeiro se o resultado não contém erros, caso contrário, falso.
    /// </summary>
    [JsonIgnore]
    public bool IsValid => Notifications.Count == 0;

    /// <summary>
    /// Coleção somente leitura de notificações.
    /// </summary>
    public IReadOnlyCollection<Notification> Notifications { get; }

    /// <summary>
    /// Dados retornados genericamente.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Construtor público para desserialização JSON.
    /// </summary>
    /// <param name="data">Dados retornados.</param>
    [JsonConstructor]
    public Result(T? data)
    {
        Data = data;
        Notifications = [];
    }

    private Result(IReadOnlyCollection<Notification> notifications)
    {
        Data = default;
        Notifications = notifications;
    }

    /// <summary>
    /// Retorna um resultado com notificações vazias.
    /// </summary>
    /// <param name="data">Dados retornados.</param>
    public static Result<T> Ok(T? data) => new(data);

    /// <summary>
    /// Retorna um resultado com notificações.
    /// </summary>
    /// <param name="notifications">Coleção de notificações.</param>
    public static Result<T> Error(IReadOnlyCollection<Notification> notifications) => new(notifications);

    /// <summary>
    /// Retorna um resultado com notificações.
    /// </summary>
    /// <param name="notifications">Array de notificações.</param>
    public static Result<T> Error(params Notification[] notifications) => new(notifications);

    /// <summary>
    /// Retorna um resultado com notificações.
    /// </summary>
    /// <param name="key">Chave da notificação.</param>
    /// <param name="message">Mensagem da notificação.</param>
    /// <returns>Resultado com notificações.</returns>
    public static Result<T> Error(string key, string message) => new([new Notification(key, message)]);
}
