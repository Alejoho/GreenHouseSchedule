using System.Collections.Generic;

namespace DataAccess.Contracts
{
    public interface IGenericRepository<Entity> where Entity : class
    {
        bool Insert(Entity entity);
        bool Update(Entity entity);
        bool Remove(int pId);
        IEnumerable<Entity> GetAll();
    }
}
