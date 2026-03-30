using Flunt.Notifications;

namespace CustomerManagementApi.Application.ValueObjects;

/// <summary>
/// Exceção lançada quando a validação de dados falha.
/// </summary>
public class ValidationException : Exception
{
    public IReadOnlyCollection<Notification> Notifications { get; }

    public ValidationException(IReadOnlyCollection<Notification> notifications)
        : base("Erro de validação.")
    {
        Notifications = notifications;
    }
}
