using CustomerManagementApi.Domain.Enums;

namespace CustomerManagementApi.Application.ResponseModel
{
    /// <summary>
    /// classe de modelo de requisição para criar um cliente
    /// </summary>
    public class CustomerResponseModel
    {
        /// <summary>
        /// Identificador do cliente
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Nome completo do cliente
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Tipo de documento do cliente (UNDEFINED = 0, CPF = 1, CNPJ = 3, PASSAPORT = 4).
        /// </summary>
        public required DocumentType DocumentType { get; set; }

        /// <summary>
        /// Número do documento do cliente.
        /// </summary>
        public required string DocumentNumber { get; set; }

        /// <summary>
        /// Endereço de e-mail do cliente.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Número de telefone do cliente.
        /// </summary>
        public string? Phone { get; set; }
    }
}
