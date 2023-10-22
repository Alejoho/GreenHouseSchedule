using System;
using System.Collections.Generic;

namespace SupportLayer.Models;

public partial class Organization
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public short MunicipalitiesId { get; set; }

    public byte TypeOfOrganizationId { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual Municipality Municipalities { get; set; } = null!;

    public virtual TypesOfOrganization TypeOfOrganization { get; set; } = null!;
}
