using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Application.Extensions;

/// <summary>
/// Fornece métodos de extensão para manipulação e validação de strings.
/// </summary>
[ExcludeFromCodeCoverage]
public static partial class StringExtension
{
    /// <summary>
    /// Remove caracteres especiais da string, deixando apenas letras e dígitos.
    /// </summary>
    /// <param name="str">A string de entrada.</param>
    /// <returns>Uma string contendo apenas letras e dígitos.</returns>
    public static string RemoveSpecialCharacters(this string str)
    {
        var cleanCharList = str.Trim().Where(@char => char.IsLetterOrDigit(@char)).ToArray();
        return new string(cleanCharList, 0, cleanCharList.Length);
    }
}