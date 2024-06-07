using System.ComponentModel.DataAnnotations.Schema;

namespace SupportLayer.Models
{
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
    }
}
