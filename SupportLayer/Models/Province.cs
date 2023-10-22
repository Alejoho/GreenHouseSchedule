using System;
using System.Collections.Generic;

namespace SupportLayer.Models;

public partial class Province
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Municipality> Municipalities { get; set; } = new List<Municipality>();
}
