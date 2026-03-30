using CustomerManagementApi.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace CustomerManagementApi.Domain.ValueObjects;

public sealed class Phone : IEquatable<Phone>
{
    public string Value { get; }

    private Phone(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidPhoneException("Telefone é obrigatório");

        var normalized = Normalize(value);

        if (!IsValid(normalized))
            throw new InvalidPhoneException("Telefone inválido");

        Value = normalized;
    }

    public string Format()
    {
        if (Value.Length == 11)
        {
            return $"({Value.Substring(0, 2)}){Value.Substring(2, 5)}-{Value.Substring(7)}";
        }
        else if (Value.Length == 10)
        {
            return $"({Value.Substring(0, 2)}){Value.Substring(2, 4)}-{Value.Substring(6)}";
        }
        throw new InvalidPhoneException("Número de telefone inválido para formatação");
    }

    public static Phone Create(string value)
        => new Phone(value);

    private static string Normalize(string phone)
        => Regex.Replace(phone, @"\D", "");

    private static bool IsValid(string phone)
        => phone.Length >= 10 && phone.Length <= 15;

    public override string ToString() => Value;

    public override bool Equals(object? obj)
        => obj is Phone other && Equals(other);

    public bool Equals(Phone? other)
        => other is not null && Value == other.Value;

    public override int GetHashCode()
        => Value.GetHashCode();
}