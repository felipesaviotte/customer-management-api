namespace CustomerManagementApi.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um CPF (Cadastro de Pessoas Físicas) válido, utilizado para identificar pessoas físicas no Brasil. O CPF é composto por 11 dígitos, onde os dois últimos são dígitos verificadores calculados a partir dos nove primeiros. Este Value Object inclui uma validação rigorosa para garantir que o CPF seja válido, evitando CPFs com todos os dígitos iguais ou com formato incorreto.
/// </summary>
public static class Cpf
{
    /// <summary>
    /// Valida um CPF, verificando se ele possui 11 dígitos, se não é composto por dígitos iguais e se os dígitos verificadores estão corretos de acordo com o algoritmo de validação do CPF.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsValid(string value)
    {
        var normalized = Normalize(value);

        if (normalized.Length != 11 || normalized.All(c => c == normalized[0]))
            return false;

        int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

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

        return digits[9] == d1 && digits[10] == d2;
    }
}
