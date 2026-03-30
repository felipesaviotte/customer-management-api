namespace CustomerManagementApi.Domain.Exceptions;

/// <summary>
/// Classe de exceção personalizada para indicar que um email é inválido.
/// </summary>
public sealed class InvalidEmailException : DomainException
{
    public InvalidEmailException(string message)
        : base(message)
    {
    }
}
