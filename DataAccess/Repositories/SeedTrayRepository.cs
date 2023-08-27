using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class SeedTrayRepository : GenericRepository, ISeedTrayRepository
    {
        public SeedTrayRepository(SowScheduleDBEntities dbContex) : base(dbContex)
        {
        }

        public SeedTrayRepository()
        {
        }

        public IEnumerable<SeedTray> GetAll()
        {
            return _sowScheduleDB.SeedTrays;
        }

        public bool Insert(SeedTray entity)
        {
            try
            {
                _sowScheduleDB.SeedTrays.Add(entity);
                _sowScheduleDB.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Remove(int pId)
        {
            try
            {
                SeedTray seedTray = _sowScheduleDB.SeedTrays.Find(pId);
                _sowScheduleDB.SeedTrays.Remove(seedTray);
                _sowScheduleDB.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(SeedTray entity)
        {
            SeedTray seedTray = _sowScheduleDB.SeedTrays.Find(entity.ID);
            if (seedTray != null)
            {
                seedTray.Name = entity.Name;
                seedTray.TotalAlveolus = entity.TotalAlveolus;
                seedTray.AlveolusLength = entity.AlveolusLength;
                seedTray.AlveolusWidth = entity.AlveolusWidth;
                seedTray.TrayLength = entity.TrayLength;
                seedTray.TrayWidth = entity.TrayWidth;
                seedTray.TrayArea = entity.TrayArea;
                seedTray.TotalAmount = entity.TotalAmount;
                seedTray.Material = entity.Material;
                seedTray.Preference = entity.Preference;
                seedTray.Active = entity.Active;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
