using log4net;

namespace SupportLayer.Models;

public partial class Organization
{
    public string MunicipalityName
    {
        get
        {
            if (Municipality != null)
            {
                return Municipality.Name;
            }

            ILog log = LogHelper.GetLogger();
            log.Warn("In a Organization object the Municipality property is null");

            return string.Empty;
        }
    }

    public string TypeOfOrganizationName
    {
        get
        {
            if (TypeOfOrganization != null)
            {
                return TypeOfOrganization.Name;
            }

            ILog log = LogHelper.GetLogger();
            log.Warn("In a Organization object the Municipality property is null");

            return string.Empty;
        }
    }

    public string ProvinceName
    {
        get
        {
            if (Municipality != null)
            {
                return Municipality.Province.Name;
            }

            ILog log = LogHelper.GetLogger();
            log.Warn("In a Organization object the Municipality property is null");

            return string.Empty;
        }
    }

    public string TypeAndOrganizationName
    {
        get
        {
            if (TypeOfOrganization != null)
            {
                return $"{TypeOfOrganization.Name} - {Name}";
            }

            ILog log = LogHelper.GetLogger();
            log.Warn("In a Organization object the Municipality property is null");

            return string.Empty;
        }
    }
}
