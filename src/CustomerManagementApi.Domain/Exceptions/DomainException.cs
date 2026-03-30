
namespace CustomerManagementApi.Domain.Exceptions
{
    /// <summary>
    /// classe base para exceções de domínio, que podem ser lançadas quando as regras de negócio são violadas. Essas exceções devem ser tratadas na camada de aplicação para fornecer feedback adequado ao usuário ou para realizar ações corretivas.
    /// </summary>
    [Serializable]
    public class DomainException : Exception
    {
        public DomainException()
        {
        }

        public DomainException(string? message) : base(message)
        {
        }

        public DomainException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}