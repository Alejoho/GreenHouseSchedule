namespace SupportLayer.Models;

public partial class Species
{
    public override string ToString()
    {
        return PropertyFormatter.FormatProperties(this);
    }
}
