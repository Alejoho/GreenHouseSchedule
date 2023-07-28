using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class DeliveryDetailRepository : IDeliveryDetailRepository
    {
        private SowScheduleDBEntities _sowScheduleDB;
        public DeliveryDetailRepository()
        {
            _sowScheduleDB = new SowScheduleDBEntities();
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
            DeliveryDetail deliveryDetail = _sowScheduleDB.DeliveryDetails.Find(pId);
            _sowScheduleDB.DeliveryDetails.Remove(deliveryDetail);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Update(DeliveryDetail entity)
        {
            DeliveryDetail deliveryDetail = _sowScheduleDB.DeliveryDetails
                .Where(x => x.DeliveryDetailID == entity.DeliveryDetailID).FirstOrDefault();
            if (deliveryDetail != null)
            {
                return true;
            }
            return false;
        }
    }
}
