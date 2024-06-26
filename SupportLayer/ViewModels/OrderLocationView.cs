using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace SupportLayer.Models;

[DebuggerDisplay(null, Name = "{Id} - {GreenHouse.Name,nq}")]
public partial class OrderLocation
{
    [NotMapped]
    public string SeedTrayName { get; set; }

    [NotMapped]
    public short RestOfSeedTraysToBeLocated
    {
        get
        {
            return Blocks != null ? (short)(SeedTrayAmount - Blocks.Sum(x => x.SeedTrayAmount)) : (short)0;
        }
    }

    [NotMapped]
    public short RestOfSeedlingToBeLocated
    {
        get
        {
            int seedTrays = Blocks != null ? (short)(SeedTrayAmount - Blocks.Sum(x => x.SeedTrayAmount)) : (short)0;
            int alveolus = SeedTray != null ? SeedTray.TotalAlveolus : 0;
            return (short)(seedTrays * alveolus);
        }
    }
}
