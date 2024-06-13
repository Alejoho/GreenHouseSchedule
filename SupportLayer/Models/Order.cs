using System.Diagnostics;

namespace SupportLayer.Models;
[DebuggerDisplay(null, Name = "{Id}")]
public partial class Order
{
    public short Id { get; set; }

    public short ClientId { get; set; }

    public byte ProductId { get; set; }

    public int AmountOfWishedSeedlings { get; set; }

    public int AmountOfAlgorithmSeedlings { get; set; }

    public DateOnly WishDate { get; set; }

    public DateOnly DateOfRequest { get; set; }

    public DateOnly EstimateSowDate { get; set; }

    public DateOnly EstimateDeliveryDate { get; set; }

    public DateOnly? RealSowDate { get; set; }

    public DateOnly? RealDeliveryDate { get; set; }

    public bool Sown { get; set; }

    public bool Delivered { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderLocation> OrderLocations { get; set; } = new List<OrderLocation>();

    public virtual Product Product { get; set; } = null!;
}
