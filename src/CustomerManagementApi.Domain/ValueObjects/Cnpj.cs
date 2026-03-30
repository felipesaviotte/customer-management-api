namespace CustomerManagementApi.Domain.ValueObjects;

/// <summary>
/// Classe estática que representa um CNPJ (Cadastro Nacional da Pessoa Jurídica) e contém um método para validar se um valor é um CNPJ válido.
/// </summary>
public static class Cnpj
{
    /// <summary>
    /// Valida um CNPJ, verificando se ele possui 14 dígitos, se não é composto por dígitos iguais e se os dígitos verificadores estão corretos de acordo com o algoritmo de validação do CNPJ.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsValid(string value)
    {
        var normalized = Normalize(value);

        if (normalized.Length != 14 || normalized.All(c => c == normalized[0]))
            return false;

        int[] mult1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        return Validate(normalized, mult1, mult2);
    }

    private static string Normalize(string value)
        => new string(value.Where(char.IsLetterOrDigit).ToArray());

    private static bool Validate(string value, int[] mult1, int[] mult2)
    {
        var digits = value.Select(c => c - '0').ToArray();

        int sum1 = mult1.Select((m, i) => m * digits[i]).Sum();
        int d1 = sum1 % 11 < 2 ? 0 : 11 - sum1 % 11;

        int sum2 = mult2.Select((m, i) => m * digits[i]).Sum();
        int d2 = sum2 % 11 < 2 ? 0 : 11 - sum2 % 11;

        return digits[12] == d1 && digits[13] == d2;
    }
}
