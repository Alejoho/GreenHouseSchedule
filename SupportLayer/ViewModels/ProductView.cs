namespace SupportLayer.Models;

public partial class Product
{
    public string SpeciesAndVariety
    {
        get => $"{Specie.Name} - {Variety}";
    }

    public override string ToString()
    {
        return PropertyFormatter.FormatProperties(this);
    }
}
