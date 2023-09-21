using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using SupportLayer.DatabaseModels;

namespace DataAccess.Repositories
{
    public class GreenHouseRepository : GenericRepository, IGreenHouseRepository
    {
        public GreenHouseRepository(SowScheduleDBEntities dbContex) : base(dbContex)
        {
        }

        public GreenHouseRepository()
        {
        }

        public IEnumerable<Greenhouse> GetAll()
        {
            return _sowScheduleDB.GreenHouses;
        }

        public bool Insert(Greenhouse entity)
        {
            try
            {
                _sowScheduleDB.GreenHouses.Add(entity);
                _sowScheduleDB.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Remove(int pId)
        {
            try
            {
                Greenhouse greenHouse = _sowScheduleDB.GreenHouses.Find(pId);
                _sowScheduleDB.GreenHouses.Remove(greenHouse);
                _sowScheduleDB.SaveChanges();
                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Greenhouse entity)
        {
            Greenhouse greenHouse = _sowScheduleDB.GreenHouses.Find(entity.ID);
            if (greenHouse != null)
            {
                greenHouse.Name = entity.Name;
                greenHouse.Description = entity.Description;
                greenHouse.Width = entity.Width;
                greenHouse.Length   = entity.Length;
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
