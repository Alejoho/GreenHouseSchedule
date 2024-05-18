namespace SupportLayer.Models;

public partial class OrderLocation
{
    public int Id { get; set; }

    public byte GreenHouseId { get; set; }

    public byte SeedTrayId { get; set; }

    public short OrderId { get; set; }

    public short SeedTrayAmount { get; set; }

    public int SeedlingAmount { get; set; }

    public DateOnly? EstimateSowDate { get; set; }

    public DateOnly? EstimateDeliveryDate { get; set; }

    public DateOnly? RealSowDate { get; set; }

    public DateOnly? RealDeliveryDate { get; set; }

    public virtual ICollection<Block> Blocks { get; set; } = new List<Block>();

    public virtual GreenHouse GreenHouse { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual SeedTray SeedTray { get; set; } = null!;
}
