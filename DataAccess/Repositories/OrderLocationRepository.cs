using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using SupportLayer.DatabaseModels;

namespace DataAccess.Repositories
{
    public class OrderLocationRepository : GenericRepository, IOrderLocationRepository
    {
        public OrderLocationRepository(SowScheduleDBEntities dbContex) : base(dbContex)
        {
        }

        public OrderLocationRepository()
        {
            
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
            OrderLocation orderLocation = _sowScheduleDB.OrderLocations.Find(entity.ID);
            if (orderLocation != null)
            {
                orderLocation.GreenHouseId = entity.GreenHouseId;
                orderLocation.SeedTrayId = entity.SeedTrayId;
                orderLocation.OrderId = entity.OrderId;
                orderLocation.SeedTrayAmount = entity.SeedTrayAmount;
                orderLocation.SeedlingAmount = entity.SeedlingAmount;
                orderLocation.SowDate = entity.SowDate;
                orderLocation.EstimateDeliveryDate = entity.EstimateDeliveryDate;
                orderLocation.RealDeliveryDate = entity.RealDeliveryDate;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}

