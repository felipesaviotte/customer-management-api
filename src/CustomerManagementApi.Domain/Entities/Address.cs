using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Domain.Entities;

/// <summary>
/// Representa o endereço do cliente.
/// </summary>
[ExcludeFromCodeCoverage]
public class Address
{
    /// <summary>
    /// Nome da rua.
    /// </summary>
    public required string Street { get; set; }

    /// <summary>
    /// Número da casa ou prédio.
    /// </summary>
    public required string Number { get; set; }

    /// <summary>
    /// Nome do bairro do cliente.
    /// </summary>
    public required string Neighborhood { get; set; }

    /// <summary>
    /// Nome da cidade do cliente.
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// Nome do estado do cliente.
    /// </summary>
    public required string State { get; set; }

    /// <summary>
    /// País do cliente.
    /// </summary>
    public required string Country { get; set; }

    /// <summary>
    /// Código postal (CEP) do cliente.
    /// </summary>
    public required string ZipCode { get; set; }

    /// <summary>
    /// Informações adicionais sobre o endereço do cliente.
    /// </summary>
    public string? Complement { get; set; }
}