using System;
using System.Collections.Generic;

namespace SupportLayer.Models;

public partial class Municipality
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public byte ProvinceId { get; set; }

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    public virtual Province Province { get; set; } = null!;
}
