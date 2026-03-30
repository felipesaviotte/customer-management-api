using System.Diagnostics.CodeAnalysis;

namespace CustomerManagementApi.Application.Extensions;

/// <summary>
/// Classe responsável por fornecer métodos de extensão para arrays.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ArrayExtension
{
    /// <summary>
    /// Extensão de array para o método Exists.
    /// </summary>
    /// <typeparam name="T">O tipo dos elementos no array.</typeparam>
    /// <param name="array">O array no qual o método será aplicado.</param>
    /// <param name="match">O predicado usado para determinar se um elemento corresponde.</param>
    /// <returns>True se um elemento correspondente for encontrado; caso contrário, false.</returns>
    public static bool Exists<T>(this T[] array, Predicate<T> match) => Array.Exists(array, match);

    /// <summary>
    /// Extensão de array para o método Find.
    /// </summary>
    /// <typeparam name="T">O tipo dos elementos no array.</typeparam>
    /// <param name="array">O array no qual o método será aplicado.</param>
    /// <param name="match">O predicado usado para localizar um elemento correspondente.</param>
    /// <returns>O primeiro elemento que corresponde ao predicado, ou o valor padrão do tipo se nenhum elemento for encontrado.</returns>
    public static T? Find<T>(this T[] array, Predicate<T> match) => Array.Find(array, match);
}
