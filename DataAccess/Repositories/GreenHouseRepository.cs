using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using SupportLayer.Models;
using DataAccess.Context;
// TODO - Fix the error in the Length propertie of GreenHouses
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

        public IEnumerable<GreenHouse> GetAll()
        {
            var output = _sowScheduleDB.GreenHouses;
            return output;
        }

        public bool Insert(GreenHouse entity)
        {
            //try
            //{
                _sowScheduleDB.GreenHouses.Add(entity);
                _sowScheduleDB.SaveChanges();
                return true;
            //}
            //catch (Exception ex)
            //{                
            //    return false;
            //}
        }

        public bool Remove(int pId)
        {
            try
            {
                GreenHouse greenHouse = _sowScheduleDB.GreenHouses.Find(pId);
                _sowScheduleDB.GreenHouses.Remove(greenHouse);
                _sowScheduleDB.SaveChanges();
                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(GreenHouse entity)
        {
            GreenHouse greenHouse = _sowScheduleDB.GreenHouses.Find(entity.Id);
            if (greenHouse != null)
            {
                greenHouse.Name = entity.Name;
                greenHouse.Description = entity.Description;
                greenHouse.Width = entity.Width;
                greenHouse.Lenght   = entity.Lenght;
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
