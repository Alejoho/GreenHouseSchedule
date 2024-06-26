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
            int seedTraysAlreadyDelivered = DeliveryDetails != null ? DeliveryDetails.Sum(x => x.SeedTrayAmountDelivered) : 0;
            int seedTraysToBeDelivered = SeedTrayAmount - seedTraysAlreadyDelivered;
            return seedTraysToBeDelivered;
        }
    }

    [NotMapped]
    public int SeedlingAmountToBeDelivered
    {
        get
        {
            int alveolus = OrderLocation != null ? OrderLocation.SeedlingAmount / OrderLocation.SeedTrayAmount : 0;
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
