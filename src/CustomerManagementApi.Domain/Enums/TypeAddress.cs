using System.ComponentModel;

namespace CustomerManagementApi.Domain.Enums;

/// <summary>
/// Enumeração para tipos de endereço
/// </summary>
public enum TypeAddress
{
    /// <summary>
    /// Localização
    /// </summary>
    [Description("L")]
    Location = 1,

    /// <summary>
    /// Faturamento
    /// </summary>
    [Description("F")]
    Billing = 2
}
