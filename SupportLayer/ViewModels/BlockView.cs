using System.ComponentModel.DataAnnotations.Schema;

namespace SupportLayer.Models
{
    public partial class Block
    {
        [NotMapped]
        public int SeedlingAmount { get; set; }

        public string BlockName
        {
            get
            {
                return "Bloque " + BlockNumber;
            }
        }
    }
}
