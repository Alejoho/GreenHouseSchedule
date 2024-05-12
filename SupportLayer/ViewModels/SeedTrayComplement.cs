using System.ComponentModel.DataAnnotations.Schema;

namespace SupportLayer.Models;

public partial class SeedTray
{
    [NotMapped]
    public bool IsSelected { get; set; } = false;
}
