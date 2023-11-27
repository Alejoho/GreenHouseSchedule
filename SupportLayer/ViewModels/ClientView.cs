namespace SupportLayer.Models;

public partial class Client
{
    public string TypeAndOrganizationName
    {
        get { return $"{Organization.TypeOfOrganization.Name} - {Organization.Name}"; }
    }
}
