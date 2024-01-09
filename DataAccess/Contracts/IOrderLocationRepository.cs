using SupportLayer.Models;

namespace DataAccess.Contracts
{
    public interface IOrderLocationRepository : IGenericRepository<OrderLocation>
    {
        IEnumerable<OrderLocation> GetByASowDateOn(DateOnly date);
    }
}
