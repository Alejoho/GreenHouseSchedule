using System.Text;

namespace SupportLayer;

public class PropertyFormatter
{
    public static string FormatProperties(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var properties = obj.GetType().GetProperties();
        StringBuilder builder = new StringBuilder();

        foreach (var property in properties)
        {
            builder.Append($"{property.Name} = {property.GetValue(obj)}, ");
        }

        if (builder.Length > 2)
        {
            builder.Length -= 2;
        }

        return builder.ToString();
    }
}
