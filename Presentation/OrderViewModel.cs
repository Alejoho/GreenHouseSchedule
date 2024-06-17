using SupportLayer.Models;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Presentation
{
    public class OrderViewModel : CollectionViewSource
    {
        public ObservableCollection<Order> Orders { get; set; }

        public OrderViewModel()
        {
            Orders = new ObservableCollection<Order>();
            Orders.Add(new Order() { Id = 1 });
            Orders.Add(new Order() { Id = 2 });
            Orders.Add(new Order() { Id = 3 });
            Orders.Add(new Order() { Id = 4 });
            this.Source = Orders;
        }
    }
}
