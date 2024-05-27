using System.ComponentModel.DataAnnotations.Schema;

namespace SupportLayer.Models;

public partial class OrderLocation
{
    [NotMapped]
    public string SeedTrayName { get; set; }
}
