using CustomerManagementApi.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Infrastructure.Mongo.Document
{
    /// <summary>
    /// Representa a identificação do cliente no mongo.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CustomerMongoDocument
    {
        /// <summary>
        /// Identificador do Cliente
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Nome completo do cliente.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Tipo de documento do cliente
        /// </summary>
        public required int DocumentType { get; set; }

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
        /// Endereço do cliente.
        /// </summary>
        //public required Address Address { get; set; }

        /// <summary>
        /// Data e hora de criação.
        /// </summary>
        public required DateTime CreatedDate { get; init; }
    }
}
