﻿using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories
{
    public class SeedTrayRepository : GenericRepository, ISeedTrayRepository
    {
        public SeedTrayRepository(SowScheduleContext dbContex) : base(dbContex)
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
            _sowScheduleDB.SeedTrays.Add(entity);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Remove(int pId)
        {
            SeedTray seedTray = _sowScheduleDB.SeedTrays.Find((byte)pId);
            _sowScheduleDB.SeedTrays.Remove(seedTray);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Update(SeedTray entity)
        {
            SeedTray seedTray = _sowScheduleDB.SeedTrays.Find(entity.Id);
            if (seedTray != null)
            {
                seedTray.Name = entity.Name;
                seedTray.TotalAlveolus = entity.TotalAlveolus;
                seedTray.AlveolusLength = entity.AlveolusLength;
                seedTray.AlveolusWidth = entity.AlveolusWidth;
                seedTray.TrayLength = entity.TrayLength;
                seedTray.TrayWidth = entity.TrayWidth;
                seedTray.TrayArea = entity.TrayArea;
                seedTray.LogicalTrayArea = entity.LogicalTrayArea;
                seedTray.TotalAmount = entity.TotalAmount;
                seedTray.Material = entity.Material;
                seedTray.Active = entity.Active;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
