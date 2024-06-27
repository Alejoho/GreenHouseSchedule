using log4net;

namespace SupportLayer.Models;

public partial class Client
{
    public string TypeAndOrganizationName
    {
        get
        {
            if (Organization != null)
            {
                return $"{Organization.TypeOfOrganization.Name} - {Organization.Name}";
            }

            ILog log = LogHelper.GetLogger();
            log.Warn("In a Client object the Organization property is null");

            return string.Empty;
        }
    }
}
