using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RestaurantManager.Helpers;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        return value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>()?
            .GetName() ?? value.ToString();
    }
}
