using System;
using System.Collections.Generic;

namespace SupportLayer.Models;

public partial class OrderDetail
{
    public short Id { get; set; }

    public short OrderId { get; set; }

    public string SeedsSource { get; set; } = null!;

    public byte? Germination { get; set; }

    public string? Description { get; set; }

    public virtual Order Order { get; set; } = null!;
}
