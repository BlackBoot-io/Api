using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Avn.Shared.Extentions;

public static class EnumExtentions
{
    public static string GetDisplayName(this Enum value)
    {
        var attribute = value.GetType().GetField(value.ToString())
            .GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault();

        if (attribute is null)
            return value.ToString();

        var propValue = attribute.GetType().GetProperty("Name").GetValue(attribute, null);
        return propValue.ToString();
    }
}
