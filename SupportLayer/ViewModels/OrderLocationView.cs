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
            return (short)(SeedTrayAmount - Blocks.Sum(x => x.SeedTrayAmount));
        }
    }

    [NotMapped]
    public short RestOfSeedlingToBeLocated
    {
        get
        {
            int seedTrays = SeedTrayAmount - Blocks.Sum(x => x.SeedTrayAmount);
            //NEXT - there is a bug here but i don't know why. I think the error is in an external code
            //when I'm going to show data the dgOrderLocations of the NewOrderWindow
            int alveolus = SeedTray.TotalAlveolus;
            return (short)(seedTrays * alveolus);
        }
    }
}
