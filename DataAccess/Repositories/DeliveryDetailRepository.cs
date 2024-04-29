using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories
{
    public class DeliveryDetailRepository : GenericRepository, IDeliveryDetailRepository
    {
        public DeliveryDetailRepository(SowScheduleContext dbContex) : base(dbContex)
        {
        }

        public DeliveryDetailRepository()
        {
        }

        public IEnumerable<DeliveryDetail> GetAll()
        {
            return _sowScheduleDB.DeliveryDetails;
        }

        public IEnumerable<DeliveryDetail> GetByADeliveryDateOn(DateOnly date)
        {
            return _sowScheduleDB.DeliveryDetails
                .Where(x => (x.Block.OrderLocation.Order.RealSowDate >= date 
                || x.Block.OrderLocation.Order.RealSowDate == null)
                    && x.DeliveryDate >= date);
        }

        public bool Insert(DeliveryDetail entity)
        {
            _sowScheduleDB.DeliveryDetails.Add(entity);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Remove(int pId)
        {
            DeliveryDetail deliveryDetail = _sowScheduleDB.DeliveryDetails.Find(pId);
            _sowScheduleDB.DeliveryDetails.Remove(deliveryDetail);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Update(DeliveryDetail entity)
        {
            DeliveryDetail deliveryDetail = _sowScheduleDB.DeliveryDetails.Find(entity.Id);
            if (deliveryDetail != null)
            {
                deliveryDetail.BlockId = entity.BlockId;
                deliveryDetail.DeliveryDate = entity.DeliveryDate;
                deliveryDetail.SeedTrayAmountDelivered = entity.SeedTrayAmountDelivered;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}

