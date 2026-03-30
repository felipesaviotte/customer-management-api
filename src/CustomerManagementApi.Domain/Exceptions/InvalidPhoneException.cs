namespace CustomerManagementApi.Domain.Exceptions;

/// <summary>
/// Classe de exceção personalizada para indicar que um email é inválido.
/// </summary>
public sealed class InvalidPhoneException : DomainException
{
    public InvalidPhoneException(string message)
        : base(message)
    {
    }
}
