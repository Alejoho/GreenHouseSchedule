using log4net;

namespace SupportLayer.Models;

public partial class Municipality
{
    public string ProvinceName
    {
        get
        {
            if (Province != null)
            {
                return Province.Name;
            }

            ILog log = LogHelper.GetLogger();
            log.Warn("In a Municipality object the Province is null");

            return string.Empty;
        }
    }

    public string Location
    {
        get
        {
            if (Province != null)
            {
                return $"{Name}, {Province.Name}";
            }

            ILog log = LogHelper.GetLogger();
            log.Warn("In a Municipality object the Province property is null");

            return string.Empty;
        }
    }
}
