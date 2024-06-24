namespace SupportLayer.Models;

public partial class DeliveryDetail
{
    public override string ToString()
    {
        return PropertyFormatter.FormatProperties(this);
    }
}
