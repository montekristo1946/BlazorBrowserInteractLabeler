using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BlazorBrowserInteractLabeler.ARM.Extension;

public static class ExtensionsEnum
{
    public static string ToDisplayName(this Enum value)
    {
        var fi = value.GetType().GetField(value.ToString());

        var attributes = (DisplayAttribute[])fi?.GetCustomAttributes(typeof(DisplayAttribute), false)!;

        if (attributes?.Any() != true)
            throw new ArgumentException(nameof(value));

        return attributes?.First()?.Name ?? string.Empty;
    }

    public static string ToDescription(this Enum value)
    {
        return ((DescriptionAttribute)Attribute.GetCustomAttribute(
            value.GetType().GetFields(BindingFlags.Public | BindingFlags.Static)
                .Single(x => x.GetValue(null)!.Equals(value)),
            typeof(DescriptionAttribute))!)?.Description ?? string.Empty;
    }

    public static T ToEnum<T>(this string data) where T : struct
    {
        if (Enum.TryParse(data, true, out T enumVariable))
        {
            if (Enum.IsDefined(typeof(T), enumVariable))
            {
                return enumVariable;
            }
        }

        return default;
    }

    public static T ToEnum<T>(this int data) where T : struct
    {
        return (T)Enum.ToObject(typeof(T), data);
    }


}


