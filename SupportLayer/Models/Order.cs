using System;
using System.Collections.Generic;

namespace SupportLayer.Models;

public partial class Order
{
    public short Id { get; set; }

    public short ClientId { get; set; }

    public byte ProductId { get; set; }

    public int AmountofWishedSeedlings { get; set; }

    public int AmountofAlgorithmSeedlings { get; set; }

    public DateTime WishDate { get; set; }

    public DateTime DateOfRequest { get; set; }

    public DateTime EstimateSowDate { get; set; }

    public DateTime EstimateDeliveryDate { get; set; }

    public DateTime? RealSowDate { get; set; }

    public DateTime? RealDeliveryDate { get; set; }

    public bool Complete { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderLocation> OrderLocations { get; set; } = new List<OrderLocation>();

    public virtual Product Product { get; set; } = null!;
}
