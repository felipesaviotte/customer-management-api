using CustomerManagementApi.Domain.Enums;
using CustomerManagementApi.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Domain.Entities;

/// <summary>
/// Representa a identificação do cliente.
/// </summary>
[ExcludeFromCodeCoverage]
public class Customer
{
    /// <summary>
    /// Identificador do Cliente
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Nome completo do cliente.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Tipo de documento do cliente
    /// </summary>
    public DocumentType DocumentType { get; set; }

    /// <summary>
    /// Número do documento do cliente.
    /// </summary>
    public Document DocumentNumber { get; set; } = null!;

    /// <summary>
    /// Endereço de e-mail do cliente.
    /// </summary>
    public Email Email { get; set; } = null!;

    /// <summary>
    /// Número de telefone do cliente.
    /// </summary>
    public Phone? Phone { get; set; }

    /// <summary>
    /// Endereço do cliente.
    /// </summary>
    //public required Address Address { get; set; }

    /// <summary>
    /// Data e hora de criação.
    /// </summary>
    public required DateTime CreatedDate { get; init; }
}
