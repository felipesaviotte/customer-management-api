using CustomerManagementApi.Domain.Exceptions;

namespace CustomerManagementApi.Domain.Exceptions;

/// <summary>
/// classe de exceção personalizada para indicar que um documento é inválido.
/// </summary>
public sealed class InvalidDocumentException : DomainException
{
    public InvalidDocumentException(string message)
        : base(message)
    {
    }
}
