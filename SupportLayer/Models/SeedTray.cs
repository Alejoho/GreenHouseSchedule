namespace SupportLayer.Models;

public partial class SeedTray
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public short TotalAlveolus { get; set; }

    public byte? AlveolusLength { get; set; }

    public byte? AlveolusWidth { get; set; }

    public decimal? TrayLength { get; set; }

    public decimal? TrayWidth { get; set; }

    public decimal? TrayArea { get; set; }

    public decimal LogicalTrayArea { get; set; }

    public short TotalAmount { get; set; }

    public string? Material { get; set; }

    public byte Preference { get; set; }

    public bool Active { get; set; }

    public virtual ICollection<OrderLocation> OrderLocations { get; set; } = new List<OrderLocation>();
}
