using System.ComponentModel.DataAnnotations.Schema;

namespace SupportLayer.Models
{
    public partial class Block
    {
        [NotMapped]
        public int SeedlingAmount
        {
            get
            {
                int alveolus = OrderLocation.SeedlingAmount / OrderLocation.SeedTrayAmount;
                return alveolus * SeedTrayAmount;
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
