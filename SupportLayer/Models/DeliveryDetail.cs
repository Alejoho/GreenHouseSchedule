using System;
using System.Collections.Generic;

namespace SupportLayer.Models;

public partial class DeliveryDetail
{
    public int Id { get; set; }

    public int BlockId { get; set; }

    public DateTime DeliveryDate { get; set; }

    public short SeedTrayAmountDelivered { get; set; }

    public virtual Block Block { get; set; } = null!;
}
