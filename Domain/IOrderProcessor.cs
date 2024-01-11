using SupportLayer.Models;

namespace Domain
{
    public interface IOrderProcessor
    {
        void DeleteOrder(int id);
        IEnumerable<Order> GetAllOrders();
        IEnumerable<Order> GetFilteredOrders(string filter);
        IEnumerable<Order> GetOrdersFromADateOn(DateOnly date);
        bool SaveOrder(Order model);
    }
}