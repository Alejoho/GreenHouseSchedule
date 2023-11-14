using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories
{
    public class ProvinceRepository : GenericRepository, IProvinceRepository
    {
        public ProvinceRepository(SowScheduleContext dbContex) : base(dbContex)
        {
        }

        public ProvinceRepository()
        {
        }

        public IEnumerable<Province> GetAll()
        {
            return _sowScheduleDB.Provinces;
        }
    }
}
