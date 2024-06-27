using log4net;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace SupportLayer.Models;

[DebuggerDisplay("Por entregar {SeedTraysAmountToBeDelivered,nq}", Name = "{Id} - Bloque {BlockNumber} - {SeedTrayAmount} seedtrays")]
public partial class Block
{
    [NotMapped]
    public int SeedTraysAmountToBeDelivered
    {
        get
        {
            int seedTraysAlreadyDelivered = 0;

            if (DeliveryDetails != null)
            {
                seedTraysAlreadyDelivered = DeliveryDetails.Sum(x => x.SeedTrayAmountDelivered);
            }
            else
            {
                ILog log = LogHelper.GetLogger();
                log.Warn("In a Block object the DeliveryDetails property is null");
            }

            int seedTraysToBeDelivered = SeedTrayAmount - seedTraysAlreadyDelivered;
            return seedTraysToBeDelivered;
        }
    }

    [NotMapped]
    public int SeedlingAmountToBeDelivered
    {
        get
        {
            int alveolus = 0;
            if (OrderLocation != null)
            {
                alveolus = OrderLocation.SeedlingAmount / OrderLocation.SeedTrayAmount;
            }
            else
            {
                ILog log = LogHelper.GetLogger();
                log.Warn("In a Block object the OrderLocation property is null");
            }

            return alveolus * SeedTraysAmountToBeDelivered;
        }
    }

    public string BlockName
    {
        get
        {
            return "Bloque " + BlockNumber;
        }
    }
}
