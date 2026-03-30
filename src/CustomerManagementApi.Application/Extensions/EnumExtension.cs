using System.ComponentModel;

namespace CustomerManagementApi.Application.Extensions;

/// <summary>
/// Métodos de extensão para enums
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// Retorna a descrição definida no atributo [Description] do enum, ou o nome do valor se não houver descrição.
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attr?.Description ?? value.ToString();
    }
}
