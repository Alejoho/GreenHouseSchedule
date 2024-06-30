using SupportLayer.Models;
using System.Collections.ObjectModel;

namespace Presentation.Forms;

public class DataContextHolderClass
{
    public ObservableCollection<Order> Orders { get; set; }
}
