namespace SupportLayer.Models;

public partial class Species
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public byte ProductionDays { get; set; }

    public decimal? WeightOf1000Seeds { get; set; }

    public int AmountOfSeedsPerHectare { get; set; }

    public decimal WeightOfSeedsPerHectare { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
