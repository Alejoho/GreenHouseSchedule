using SupportLayer.Models;
using System.Collections.ObjectModel;

namespace Presentation.Forms;

//LATER -Maybe remove it and do these things the way they should be done
public class DataContextHolderClass
{
    public ObservableCollection<Order> Orders { get; set; }
}
