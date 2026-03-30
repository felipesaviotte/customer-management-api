using Flunt.Notifications;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Application.ResponseModel;

/// <summary>
/// Modelo de erro
/// </summary>
[ExcludeFromCodeCoverage]
public class ErrorModel
{
    /// <summary>
    /// Lista de erros
    /// </summary>
    public IReadOnlyCollection<Notification> Errors { get; } = [];

    /// <summary>
    /// Constrói o modelo de erro a partir de notificações
    /// </summary>
    /// <param name="notifications">Coleção de notificações</param>
    public ErrorModel(IReadOnlyCollection<Notification> notifications) => Errors = notifications;

    /// <summary>
    /// Constrói o modelo de erro a partir de uma notificação
    /// </summary>
    /// <param name="key">Chave do erro</param>
    /// <param name="message">Mensagem do erro</param>
    public ErrorModel(string key, string message) => Errors =
    [
        new Notification { Key = key, Message = message }
    ];
}