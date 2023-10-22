using System;
using System.Collections.Generic;

namespace SupportLayer.Models;

public partial class Product
{
    public byte Id { get; set; }

    public byte SpecieId { get; set; }

    public string Variety { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Species Specie { get; set; } = null!;
}
