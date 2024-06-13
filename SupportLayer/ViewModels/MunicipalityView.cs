namespace SupportLayer.Models;

public partial class Municipality
{
    public string ProvinceName
    {
        get { return Province.Name; }
    }

    public string Location
    {
        get { return $"{Name}, {Province.Name}"; }
    }
}
