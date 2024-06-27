using log4net;
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
            if (Blocks != null)
            {
                return (short)(SeedTrayAmount - Blocks.Sum(x => x.SeedTrayAmount));
            }

            ILog log = LogHelper.GetLogger();
            log.Warn("In a OrderLocation object the Blocks property is null");

            return 0;
        }
    }

    [NotMapped]
    public short RestOfSeedlingToBeLocated
    {
        get
        {
            if (Blocks != null)
            {
                int seedTrays = Blocks != null ? (short)(SeedTrayAmount - Blocks.Sum(x => x.SeedTrayAmount)) : (short)0;
                int alveolus = SeedTray != null ? SeedTray.TotalAlveolus : 0;
                return (short)(seedTrays * alveolus);
            }

            ILog log = LogHelper.GetLogger();
            log.Warn("In a OrderLocation object the Blocks property is null");

            return 0;
        }
    }
}
