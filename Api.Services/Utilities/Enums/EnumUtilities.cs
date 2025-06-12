using System.ComponentModel;
using System.Reflection;

namespace Api.Services.Utilities.Enums;

public static class EnumHelper
{
    public static Dictionary<string, int> ToDisplayDictionary<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .ToDictionary(
                       e => GetEnumDisplayName(e),
                       e => Convert.ToInt32(e)
                   );
    }

    public static string GetEnumDisplayName<T>(T value) where T : Enum
    {
        var field = typeof(T).GetField(value.ToString());
        var descriptionAttr = field?.GetCustomAttribute<DescriptionAttribute>();

        return descriptionAttr?.Description ?? value.ToString();
    }

    public static string GetEnumDisplayNameFromInt<T>(int value) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
            return "Unknown";

        var enumValue = (T)Enum.ToObject(typeof(T), value);
        return GetEnumDisplayName(enumValue);
    }


    // public static string GetEnumDescription<TEnum>(TEnum value) where TEnum : struct, Enum
    // {
    //     FieldInfo fi = value.GetType().GetField(value.ToString());

    //     if (fi != null)
    //     {
    //         DescriptionAttribute[] attributes =
    //             (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

    //         if (attributes.Length > 0)
    //             return attributes[0].Description;
    //     }

    //     return value.ToString();
    // }

}
