using CustomerManagementApi.Domain.Enums;
using CustomerManagementApi.Domain.Exceptions;

namespace CustomerManagementApi.Domain.ValueObjects;

/// <summary>
/// Classe que representa um documento de identificação, como CPF, CNPJ ou passaporte. Ela é imutável e implementa a interface IEquatable para comparação de igualdade. O método Create é responsável por validar o formato do documento e criar uma instância da classe, lançando uma exceção caso o documento seja inválido.
/// </summary>
public sealed class Document : IEquatable<Document>
{
    public string Value { get; }
    public DocumentType Type { get; }

    private Document(string value, DocumentType type)
    {
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Cria uma instância de Document a partir de uma string de entrada, validando se é um CPF, CNPJ ou passaporte. O método normaliza a string removendo caracteres não alfanuméricos antes de realizar as validações. Se o documento for válido, ele retorna uma nova instância de Document; caso contrário, lança uma InvalidDocumentException com uma mensagem apropriada.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDocumentException"></exception>
    public static Document Create(string value, DocumentType type)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidDocumentException("Documento é obrigatório.");

        var normalized = Normalize(value);

        switch (type)
        {
            case DocumentType.UNDEFINED:
                throw new InvalidDocumentException("Tipo de documento é obrigatório.");
            case DocumentType.CPF:
                if (Cpf.IsValid(normalized))
                    return new Document(normalized, DocumentType.CPF);
                break;
            case DocumentType.CNPJ:
                if (Cnpj.IsValid(normalized))
                    return new Document(normalized, DocumentType.CNPJ);
                break;
            case DocumentType.PASSAPORT:
                if (Passport.IsValid(normalized))
                    return new Document(normalized, DocumentType.PASSAPORT);
                break;
        }

        throw new InvalidDocumentException("Documento inválido.");
    }

    private static string Normalize(string value)
        => new string(value.Where(char.IsLetterOrDigit).ToArray());

    public bool Equals(Document? other)
        => other is not null &&
           Value == other.Value &&
           Type == other.Type;

    public override bool Equals(object? obj)
        => obj is Document other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Value, Type);

    public static bool operator ==(Document? left, Document? right)
        => Equals(left, right);

    public static bool operator !=(Document? left, Document? right)
        => !Equals(left, right);

}
