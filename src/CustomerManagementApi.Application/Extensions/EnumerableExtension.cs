using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Application.Extensions;

/// <summary>
/// Classe responsável por fornecer métodos de extensão para enumeráveis.
/// </summary>
[ExcludeFromCodeCoverage]
public static class EnumerableExtension
{
    /// <summary>
    /// Determina se o enumerável é nulo ou vazio.
    /// </summary>
    /// <typeparam name="TType">O tipo dos elementos no enumerável.</typeparam>
    /// <param name="enumerable">O enumerável a ser verificado.</param>
    /// <returns>True se o enumerável for nulo ou vazio; caso contrário, false.</returns>
    public static bool IsNullOrEmpty<TType>([NotNullWhen(false)] this IEnumerable<TType>? enumerable)
    {
        if (enumerable is null)
            return true;

        return !enumerable.Any();
    }

    /// <summary>
    /// Determina se um objeto é um enumerável.
    /// </summary>
    /// <param name="object">O objeto a ser verificado.</param>
    /// <returns>True se o objeto for um enumerável; caso contrário, false.</returns>
    /// <exception cref="ArgumentNullException">Lançada se o objeto for nulo.</exception>
    public static bool IsEnumerable(this object @object)
    {
        ThrowExceptionWhenSourceArgumentIsNull(@object);

        var objectType = @object.GetType();
        var result = objectType.IsGenericType && objectType.GetInterfaces().Exists(@interface => @interface.Name == nameof(IEnumerable));
        return result;
    }

    /// <summary>
    /// Determina se um objeto é um enumerável e se a sequência está vazia.
    /// </summary>
    /// <param name="object">O objeto a ser verificado.</param>
    /// <returns>True se o objeto for um enumerável e não tiver elementos; caso contrário, false.</returns>
    /// <exception cref="ArgumentNullException">Lançada se o objeto for nulo.</exception>
    public static bool IsEmptyEnumerable(this object @object)
    {
        ThrowExceptionWhenSourceArgumentIsNull(@object);
        return !@object.IsEnumerable() && Enumerable.Any((dynamic)@object);
    }

    /// <summary>
    /// Remove todas as propriedades nulas e retorna um objeto não nulo na resposta.
    /// </summary>
    /// <typeparam name="TType">O tipo dos elementos no enumerável.</typeparam>
    /// <param name="enumerable">O enumerável a ser filtrado.</param>
    /// <returns>Um enumerável contendo apenas elementos não nulos.</returns>
    public static IEnumerable<TType> WhereNotNull<TType>(this IEnumerable<TType?> enumerable) =>
        enumerable.Where(prop => prop is not null).Select(prop => prop!);

    private static void ThrowExceptionWhenSourceArgumentIsNull(object @object)
    {
        if (@object is null)
            throw new ArgumentNullException(nameof(@object), "O objeto de origem é nulo.");
    }

    /// <summary>
    /// Verifica se uma coleção somente leitura possui elementos.
    /// </summary>
    /// <typeparam name="TType">O tipo dos elementos na coleção.</typeparam>
    /// <param name="enumrable">A coleção a ser verificada.</param>
    /// <returns>True se a coleção possuir elementos; caso contrário, false.</returns>
    public static bool HasAny<TType>(this IReadOnlyCollection<TType> enumrable) => enumrable.Count > 0;

    /// <summary>
    /// Verifica se uma lista possui elementos.
    /// </summary>
    /// <typeparam name="TType">O tipo dos elementos na lista.</typeparam>
    /// <param name="enumrable">A lista a ser verificada.</param>
    /// <returns>True se a lista possuir elementos; caso contrário, false.</returns>
    public static bool HasAny<TType>(this List<TType> enumrable) => enumrable.Count > 0;
}