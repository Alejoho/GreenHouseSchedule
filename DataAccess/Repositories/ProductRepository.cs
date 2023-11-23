using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SupportLayer.Models;
using DataAccess.Context;

namespace DataAccess.Repositories
{
    public class ProductRepository : GenericRepository, IProductRepository
    {
        public ProductRepository(SowScheduleContext dbContex) : base(dbContex)
        {
        }

        public IEnumerable<Product> GetAll()
        {
            return _sowScheduleDB.Products;
        }

        public bool Insert(Product entity)
        {
                _sowScheduleDB.Products.Add(entity);
                _sowScheduleDB.SaveChanges();
                return true;
        }

        public bool Remove(int pId)
        {
                Product product = _sowScheduleDB.Products.Find(pId);
                _sowScheduleDB.Products.Remove(product);
                _sowScheduleDB.SaveChanges();
                return true;
        }

        public bool Update(Product entity)
        {
            Product product = _sowScheduleDB.Products.Find(entity.Id);
            if(product!=null)
            {
                product.SpecieId = entity.SpecieId;
                product.Variety = entity.Variety;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
