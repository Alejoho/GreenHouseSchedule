namespace SupportLayer.Models;

public partial class Organization
{
    public string MunicipalityName
    {
        get { return Municipality.Name; }
    }

    public string TypeOfOrganizationName
    {
        get { return TypeOfOrganization.Name; }
    }

    public string ProvinceName
    {
        get { return Municipality.Province.Name; }
    }

    public string TypeAndOrganizationName
    {
        get { return $"{TypeOfOrganization.Name} - {Name}"; }
    }
}
