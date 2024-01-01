using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SupportLayer.Models;

public partial class SeedTray
{
    [NotMapped]
    public bool IsSelected { get; set; } = false;
}
