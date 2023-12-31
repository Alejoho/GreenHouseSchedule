﻿using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SupportLayer.Models;
using DataAccess.Context;

namespace DataAccess.Repositories
{
    public class OrderDetailRepository : GenericRepository, IOrderDetailRepository
    {
        public OrderDetailRepository(SowScheduleContext dbContex) : base(dbContex) 
        {
        }

        public IEnumerable<OrderDetail> GetAll()
        {
            return _sowScheduleDB.OrderDetails;
        }

        public bool Insert(OrderDetail entity)
        {
                _sowScheduleDB.OrderDetails.Add(entity);
                _sowScheduleDB.SaveChanges();
                return true;
        }

        public bool Remove(int pId)
        {
                OrderDetail orderDetail = _sowScheduleDB.OrderDetails.Find(pId);
                _sowScheduleDB.OrderDetails.Remove(orderDetail);
                _sowScheduleDB.SaveChanges();
                return true;
        }

        public bool Update(OrderDetail entity)
        {
            OrderDetail orderDetail = _sowScheduleDB.OrderDetails.Find(entity.Id);
            if (orderDetail != null)
            {
                orderDetail.OrderId = entity.OrderId;
                orderDetail.SeedsSource = entity.SeedsSource;
                orderDetail.Germination = entity.Germination;
                orderDetail.Description = entity.Description;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
