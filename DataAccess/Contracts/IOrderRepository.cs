using SupportLayer.Models;

namespace DataAccess.Contracts
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        IEnumerable<Order> GetByARealSowDateOn(DateOnly date);
        IEnumerable<Order> GetIncompleteBeforeADate(DateOnly date);
        IEnumerable<Order> GetSownsWithoutPlace();
    }
}
