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
            int seedTraysAlreadyDelivered = DeliveryDetails.Sum(x => x.SeedTrayAmountDelivered);
            int seedTraysToBeDelivered = SeedTrayAmount - seedTraysAlreadyDelivered;
            return seedTraysToBeDelivered;
        }
    }

    [NotMapped]
    public int SeedlingAmountToBeDelivered
    {
        get
        {
            int alveolus = OrderLocation.SeedlingAmount / OrderLocation.SeedTrayAmount;
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

    public override string ToString()
    {
        return PropertyFormatter.FormatProperties(this);
    }
}
