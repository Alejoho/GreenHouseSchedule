using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private SowScheduleDBEntities _sowScheduleDB;
        public OrderRepository()
        {
            _sowScheduleDB = new SowScheduleDBEntities();
        }

        public IEnumerable<Order> GetAll()
        {
            return _sowScheduleDB.Orders;
        }

        public bool Insert(Order entity)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int pId)
        {
            throw new NotImplementedException();
        }

        public bool Update(Order entity)
        {
            throw new NotImplementedException();
        }
    }
}
