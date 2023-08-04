using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class OrderLocationRepository : IOrderLocationRepository
    {
        private SowScheduleDBEntities _sowScheduleDB;
        public OrderLocationRepository()
        {
            _sowScheduleDB = new SowScheduleDBEntities();
        }

        public IEnumerable<OrderLocation> GetAll()
        {
            return _sowScheduleDB.OrderLocations;
        }

        public bool Insert(OrderLocation entity)
        {
            try
            {
                _sowScheduleDB.OrderLocations.Add(entity);
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
            OrderLocation orderLocation = _sowScheduleDB.OrderLocations.Find(pId);
            _sowScheduleDB.OrderLocations.Remove(orderLocation);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Update(OrderLocation entity)
        {
            OrderLocation orderLocation = _sowScheduleDB.OrderLocations
                .Where(x => x.ID == entity.ID).FirstOrDefault();
            if (orderLocation != null)
            {
                return true;
            }
            return false;
        }
    }
}

