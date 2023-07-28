using DataAccess.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class SeedTrayRepository : ISeedTrayRepository
    {
        private SowScheduleDBEntities _sowScheduleDB;
        public SeedTrayRepository()
        {
            _sowScheduleDB = new SowScheduleDBEntities();
        }

        public IEnumerable<SeedTray> GetAll()
        {
            return _sowScheduleDB.SeedTrays.ToList();
        }

        public bool Insert(SeedTray entity)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(int pId)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(SeedTray entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
