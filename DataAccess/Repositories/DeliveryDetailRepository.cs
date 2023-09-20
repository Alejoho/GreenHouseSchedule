using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportLayer.DatabaseModels;

namespace DataAccess.Repositories
{
    public class DeliveryDetailRepository : GenericRepository, IDeliveryDetailRepository
    {
        public DeliveryDetailRepository(SowScheduleDBEntities dbContex) : base(dbContex)
        {
        }

        public DeliveryDetailRepository()
        {
        }

        public IEnumerable<DeliveryDetail> GetAll()
        {
            return _sowScheduleDB.DeliveryDetails;
        }

        public bool Insert(DeliveryDetail entity)
        {
            try
            {
                _sowScheduleDB.DeliveryDetails.Add(entity);
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
            try
            {
                DeliveryDetail deliveryDetail = _sowScheduleDB.DeliveryDetails.Find(pId);
                _sowScheduleDB.DeliveryDetails.Remove(deliveryDetail);
                _sowScheduleDB.SaveChanges();
                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(DeliveryDetail entity)
        {
            DeliveryDetail deliveryDetail = _sowScheduleDB.DeliveryDetails.Find(entity.ID);                
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

