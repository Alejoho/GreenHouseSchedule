namespace SupportLayer.Models;

public partial class GreenHouse
{
    public override string ToString()
    {
        return PropertyFormatter.FormatProperties(this);
    }
}
