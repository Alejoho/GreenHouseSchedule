using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupportLayer.Models
{
    public partial class Order
    {
        [NotMapped]
        public ObservableCollection<OrderLocation> OrderLocationsView { get; set; }
    }
}
