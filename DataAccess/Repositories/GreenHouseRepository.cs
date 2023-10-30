using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories
{
    public class GreenHouseRepository : GenericRepository, IGreenHouseRepository
    {
        public GreenHouseRepository(SowScheduleContext dbContex) : base(dbContex)
        {
        }

        public GreenHouseRepository()
        {
        }

        public IEnumerable<GreenHouse> GetAll()
        {
            var output = _sowScheduleDB.GreenHouses;
            return output;
        }

        public bool Insert(GreenHouse entity)
        {
            _sowScheduleDB.GreenHouses.Add(entity);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Remove(int pId)
        {
            GreenHouse greenHouse = _sowScheduleDB.GreenHouses.Find((byte)pId);
            _sowScheduleDB.GreenHouses.Remove(greenHouse);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Update(GreenHouse entity)
        {
            GreenHouse greenHouse = _sowScheduleDB.GreenHouses.Find(entity.Id);
            if (greenHouse != null)
            {
                greenHouse.Name = entity.Name;
                greenHouse.Description = entity.Description;
                greenHouse.Width = entity.Width;
                greenHouse.Length = entity.Length;
                greenHouse.GreenHouseArea = entity.GreenHouseArea;
                greenHouse.SeedTrayArea = entity.SeedTrayArea;
                greenHouse.AmountOfBlocks = entity.AmountOfBlocks;
                greenHouse.Active = entity.Active;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
