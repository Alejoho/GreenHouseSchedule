using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportLayer.Models;
using DataAccess.Context;

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
            return _sowScheduleDB.DeliveryDetails.Where(x => x.DeliveryDate >= date);
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
                deliveryDetail.BlockId=entity.BlockId;
                deliveryDetail.DeliveryDate = entity.DeliveryDate;
                deliveryDetail.SeedTrayAmountDelivered = entity.SeedTrayAmountDelivered;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}

