using System.ComponentModel;

namespace CustomerManagementApi.Domain.Enums;

/// <summary>
/// Situação do cliente.
/// </summary>
/// <remarks>
/// Valores válidos:
/// <list type="bullet">
/// <item><description><c>INDEFINIDO</c> (0) — Situação não definida</description></item>
/// <item><description><c>ATIVO</c> (1) — Cliente ativo</description></item>
/// <item><description><c>INATIVO</c> (2) — Cliente inativo</description></item>
/// <item><description><c>BLOQUEADO</c> (3) — Cliente bloqueado</description></item>
/// </list>
/// </remarks>
public enum CustomerStatus
{
    /// <summary>
    /// Situação não definida
    /// </summary>
    [Description("Indefinido")]
    INDEFINIDO = 0,

    /// <summary>
    /// Cliente ativo
    /// </summary>
    [Description("Ativo")]
    ATIVO = 1,

    /// <summary>
    /// Cliente inativo
    /// </summary>
    [Description("Inativo")]
    INATIVO = 2,

    /// <summary>
    /// Cliente bloqueado
    /// </summary>
    [Description("Bloqueado")]
    BLOQUEADO = 3
}
