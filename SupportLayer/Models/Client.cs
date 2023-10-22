using System;
using System.Collections.Generic;

namespace SupportLayer.Models;

public partial class Client
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public string? NickName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? OtherNumber { get; set; }

    public short OrganizationId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Organization Organization { get; set; } = null!;
}
