using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories
{
    public class ProductRepository : GenericRepository, IProductRepository
    {
        public ProductRepository(SowScheduleContext dbContex) : base(dbContex)
        {
        }

        public ProductRepository()
        {
        }

        public IEnumerable<Product> GetAll()
        {
            return _sowScheduleDB.Products;
        }

        //LATER - Make the test for this method
        public Product GetById(int pId)
        {
            return _sowScheduleDB.Products.Find((byte)pId);
        }

        public bool Insert(Product entity)
        {
            _sowScheduleDB.Products.Add(entity);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Remove(int pId)
        {
            Product product = _sowScheduleDB.Products.Find((byte)pId);
            _sowScheduleDB.Products.Remove(product);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Update(Product entity)
        {
            Product product = _sowScheduleDB.Products.Find(entity.Id);
            if (product != null)
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
