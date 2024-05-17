namespace SupportLayer.Models;

public partial class Block
{
    public int Id { get; set; }

    public int OrderLocationId { get; set; }

    public byte BlockNumber { get; set; }

    public short SeedTrayAmount { get; set; }

    public byte NumberWithinTheBlock { get; set; }

    public virtual ICollection<DeliveryDetail> DeliveryDetails { get; set; } = new List<DeliveryDetail>();

    public virtual OrderLocation OrderLocation { get; set; } = null!;
}
