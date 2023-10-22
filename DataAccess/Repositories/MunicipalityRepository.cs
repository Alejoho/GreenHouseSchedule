using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using SupportLayer.Models;
using DataAccess.Context;

namespace DataAccess.Repositories
{
    public class MunicipalityRepository : GenericRepository, IMunicipalityRepository
    {
        public MunicipalityRepository(SowScheduleDBEntities dbContex) : base(dbContex)
        {
        }

        public IEnumerable<Municipality> GetAll()
        {
            return _sowScheduleDB.Municipalities;
        }

        public bool Insert(Municipality entity)
        {
            try
            {
                _sowScheduleDB.Municipalities.Add(entity);
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
                Municipality municipality = _sowScheduleDB.Municipalities.Find(pId);
                _sowScheduleDB.Municipalities.Remove(municipality);
                _sowScheduleDB.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(Municipality entity)
        {
            Municipality municipality = _sowScheduleDB.Municipalities.Find(entity.Id);
            if (municipality != null)
            {
                municipality.Name = entity.Name;
                municipality.ProvinceId = entity.ProvinceId;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
