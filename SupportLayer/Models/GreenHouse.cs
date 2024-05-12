namespace SupportLayer.Models;

public partial class GreenHouse
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Width { get; set; }

    public decimal? Length { get; set; }

    public decimal? GreenHouseArea { get; set; }

    public decimal SeedTrayArea { get; set; }

    public byte AmountOfBlocks { get; set; }

    public bool Active { get; set; }

    public virtual ICollection<OrderLocation> OrderLocations { get; set; } = new List<OrderLocation>();
}
