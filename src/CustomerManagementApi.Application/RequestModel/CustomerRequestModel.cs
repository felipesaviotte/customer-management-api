using CustomerManagementApi.Domain.Enums;

namespace CustomerManagementApi.Application.RequestModel
{
    /// <summary>
    /// classe de modelo de requisição para criar um cliente
    /// </summary>
    public class CustomerRequestModel
    {
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

        /// <summary>
        /// Situação do cliente (INDEFINIDO = 0, ATIVO = 1, INATIVO = 2, BLOQUEADO = 3).
        /// </summary>
        public CustomerStatus Status { get; set; }
    }
}
