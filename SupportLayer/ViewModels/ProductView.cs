namespace SupportLayer.Models;

public partial class Product
{
    public string SpeciesAndVariety 
    {
        get => $"{Specie.Name} - {Variety}"; 
    }
}
