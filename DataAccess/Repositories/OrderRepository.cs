﻿using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories
{
    public class OrderRepository : GenericRepository, IOrderRepository
    {

        public OrderRepository(SowScheduleContext dbContex) : base(dbContex)
        {
        }

        public OrderRepository()
        {
        }

        public IEnumerable<Order> GetAll()
        {
            return _sowScheduleDB.Orders;
        }

        public IEnumerable<Order> GetByARealSowDateOn(DateOnly date)
        {
            return _sowScheduleDB.Orders.Where(x => x.RealSowDate >= date || x.RealSowDate == null);
        }
        //LATER - Make the test for this method.
        public IEnumerable<Order> GetIncompleteBeforeADate(DateOnly date)
        {
            return _sowScheduleDB.Orders.Where(x => x.Sown == false && x.EstimateSowDate <= date);
        }
        //LATER - Make the test for this method.
        public IEnumerable<Order> GetSownsWithoutPlace()
        {
            return _sowScheduleDB.Orders.Where(x => x.OrderLocations.Any(y => y.RealSowDate != null && y.GreenHouseId == 0));
        }
        //LATER - Make the test for this method.
        public IEnumerable<Order> GetReadyToDeliver(DateOnly date)
        {
            return _sowScheduleDB.Orders.Where(x => x.EstimateDeliveryDate <= date && x.Delivered == false);
        }
        //LATER - Make the test for this method.
        public IEnumerable<Order> GetSownsWithPlace()
        {
            return _sowScheduleDB.Orders.Where(x => x.Delivered == false && x.OrderLocations.Any(y => y.GreenHouseId > 0));
        }

        public bool Insert(Order entity)
        {
            _sowScheduleDB.Orders.Add(entity);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Remove(int pId)
        {
            Order order = _sowScheduleDB.Orders.Find((short)pId);
            _sowScheduleDB.Orders.Remove(order);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Update(Order entity)
        {
            Order order = _sowScheduleDB.Orders.Find(entity.Id);
            if (order != null)
            {
                order.Id = entity.Id;
                order.ClientId = entity.ClientId;
                order.ProductId = entity.ProductId;
                order.AmountOfWishedSeedlings = entity.AmountOfWishedSeedlings;
                order.AmountOfAlgorithmSeedlings = entity.AmountOfAlgorithmSeedlings;
                order.WishDate = entity.WishDate;
                order.DateOfRequest = entity.DateOfRequest;
                order.EstimateSowDate = entity.EstimateSowDate;
                order.EstimateDeliveryDate = entity.EstimateDeliveryDate;
                order.RealSowDate = entity.RealSowDate;
                order.RealDeliveryDate = entity.RealDeliveryDate;
                order.Sown = entity.Sown;
                order.Delivered = entity.Delivered;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
