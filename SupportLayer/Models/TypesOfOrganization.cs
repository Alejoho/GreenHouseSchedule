namespace SupportLayer.Models;

public partial class TypesOfOrganization
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();
}
