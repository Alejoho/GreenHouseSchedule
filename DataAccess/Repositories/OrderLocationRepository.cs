using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories
{
    public class OrderLocationRepository : GenericRepository, IOrderLocationRepository
    {
        public OrderLocationRepository(SowScheduleContext dbContex) : base(dbContex)
        {
        }

        public OrderLocationRepository()
        {

        }

        public IEnumerable<OrderLocation> GetAll()
        {
            return _sowScheduleDB.OrderLocations;
        }

        public IEnumerable<OrderLocation> GetByASowDateOn(DateOnly date)
        {
            return _sowScheduleDB.OrderLocations
                .Where(x => x.Order.RealSowDate >= date 
                && (x.SowDate >= date || x.SowDate == null));
        }

        public bool Insert(OrderLocation entity)
        {
            _sowScheduleDB.OrderLocations.Add(entity);
            _sowScheduleDB.SaveChanges();
            return true;
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
            OrderLocation orderLocation = _sowScheduleDB.OrderLocations.Find(entity.Id);
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

