using System.ComponentModel;

namespace CustomerManagementApi.Domain.Enums;

/// <summary>
/// Tipos de documento utilizados para identificação do cliente.
/// </summary>
/// <remarks>
/// Valores válidos:
/// <list type="bullet">
/// <item><description><c>UNDEFINED</c> (0) — Tipo de documento não definido</description></item>
/// <item><description><c>CPF</c> (1) — Cadastro de Pessoa Física (11 dígitos)</description></item>
/// <item><description><c>CNPJ</c> (3) — Cadastro Nacional da Pessoa Jurídica (14 dígitos)</description></item>
/// <item><description><c>PASSAPORT</c> (4) — Passaporte internacional</description></item>
/// </list>
/// </remarks>
public enum DocumentType
{
    /// <summary>
    /// Documento indefinido
    /// </summary>
    [Description("Indefinido")]
    UNDEFINED = 0,

    /// <summary>
    /// CPF
    /// </summary>
    [Description("CPF")]
    CPF = 1,

    /// <summary>
    /// CNPJ
    /// </summary>    
    [Description("CNPJ")] 
    CNPJ = 3,

    /// <summary>
    /// Passaporte
    /// </summary>   
    [Description("Passaporte")] 
    PASSAPORT = 4
}
