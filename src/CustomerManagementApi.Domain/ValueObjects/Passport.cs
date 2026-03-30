namespace CustomerManagementApi.Domain.ValueObjects;

/// <summary>
/// Classe de valor que representa um passaporte, com validação de formato e comprimento.
/// </summary>
public static class Passport
{
    /// <summary>
    /// Valida se o valor do passaporte atende aos critérios de formato e comprimento (entre 6 e 20 caracteres).
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsValid(string value)
        => value.Length >= 6 && value.Length <= 20;
}
