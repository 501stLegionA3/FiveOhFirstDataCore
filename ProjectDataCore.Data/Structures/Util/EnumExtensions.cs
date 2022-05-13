using System.ComponentModel;
using System.Reflection;

namespace ProjectDataCore.Data.Structures.Util;
public static class EnumExtensions
{
    public static string AsFull<T>(this T value) where T : Enum
    {
        if (value is null) return "";

        var type = value.GetType();
        var name = Enum.GetName(type, value);

        if (name is null) return "";

        return type?.GetField(name)
            ?.GetCustomAttribute<DescriptionAttribute>()
            ?.Description ?? "";
    }
}
