using CustomerManagementApi.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace CustomerManagementApi.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException("Email é obrigatório.");

        var normalized = Normalize(value);

        if (!EmailRegex.IsMatch(normalized))
            throw new InvalidEmailException("Formato de email inválido.");

        return new Email(normalized);
    }

    private static string Normalize(string value)
        => value.Trim().ToLowerInvariant();

    public override string ToString() => Value;

    public bool Equals(Email? other)
        => other is not null && Value == other.Value;

    public override bool Equals(object? obj)
        => obj is Email other && Equals(other);

    public override int GetHashCode()
        => Value.GetHashCode();
}