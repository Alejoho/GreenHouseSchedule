using log4net;

namespace SupportLayer.Models;

public partial class Product
{
    public string SpeciesAndVariety
    {
        get
        {
            if (Specie != null)
            {
                return $"{Specie.Name} - {Variety}";
            }

            ILog log = LogHelper.GetLogger();
            log.Warn("In a Product object the Specie property is null");

            return string.Empty;
        }
    }
}
