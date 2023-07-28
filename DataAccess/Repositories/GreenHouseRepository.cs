using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;

namespace DataAccess.Repositories
{
    public class GreenHouseRepository : IGreenHouseRepository
    {
        private SowScheduleDBEntities _sowScheduleDB;
        public GreenHouseRepository()
        {
            _sowScheduleDB = new SowScheduleDBEntities();
        }

        public IEnumerable<Greenhouse> GetAll()
        {
            //try
            //{
            return _sowScheduleDB.GreenHouses;
            //}
            //catch (Exception ex)
            //{

            //}
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
            Greenhouse greenHouse = _sowScheduleDB.GreenHouses.Find(pId);
            _sowScheduleDB.GreenHouses.Remove(greenHouse);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Update(Greenhouse entity)
        {
            Greenhouse greenHouse = _sowScheduleDB.GreenHouses
                .Where(x => x.GreenHouseID == entity.GreenHouseID).FirstOrDefault();
            if(greenHouse != null)
            {
                return true;
            }
            return false;
        }
    }
}
