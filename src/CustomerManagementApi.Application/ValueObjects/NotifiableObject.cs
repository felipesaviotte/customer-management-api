using Flunt.Notifications;
using Flunt.Validations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CustomerManagementApi.Application.ValueObjects;

/// <summary>
/// Classe base abstrata para objetos que podem conter notificações de validação.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class NotifiableObject
{
    private readonly List<Notification> _notifications = [];

    /// <summary>
    /// Verdadeiro se houver notificações, caso contrário, falso.
    /// </summary>
    [JsonIgnore]
    public bool HasNotifications => _notifications.Count != 0;

    /// <summary>
    /// Valida o objeto.
    /// </summary>
    /// <returns>Retorna verdadeiro se o objeto for válido, caso contrário, falso.</returns>
    public abstract bool IsValid();

    /// <summary>
    /// Lista de notificações.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyCollection<Notification> Notifications
    {
        get
        {
            return _notifications;
        }
    }

    /// <summary>
    /// Adiciona notificações de um contrato.
    /// </summary>
    /// <typeparam name="T">Tipo do contrato.</typeparam>
    /// <param name="contract">Contrato contendo as notificações.</param>
    public void AddNotifications<T>(Contract<T> contract)
    {
        _notifications.AddRange(contract.Notifications);
    }

    /// <summary>
    /// Adiciona notificações de um objeto notificável.
    /// </summary>        
    /// <param name="notifiable">Objeto notificável.</param>
    public void AddNotifications(NotifiableObject notifiable)
    {
        _notifications.AddRange(notifiable.Notifications);
    }

    /// <summary>
    /// Adiciona notificações de múltiplos objetos notificáveis.
    /// </summary>       
    /// <param name="notifiables">Array de objetos notificáveis.</param>
    public void AddNotifications(params NotifiableObject[] notifiables)
    {
        _notifications.AddRange(notifiables.SelectMany(notifiable => notifiable.Notifications));
    }
}