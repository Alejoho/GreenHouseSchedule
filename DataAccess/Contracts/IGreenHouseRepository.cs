using SupportLayer.Models;

namespace DataAccess.Contracts
{
    public interface IGreenHouseRepository : IGenericRepository<GreenHouse>
    {
        public IEnumerable<GreenHouse> GetOnlyActive();
    }
}
