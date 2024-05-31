using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories;

public class BlockRepository : GenericRepository, IBlockRepository
{
    public BlockRepository(SowScheduleContext dbContex) : base(dbContex)
    {
    }

    public BlockRepository()
    {
    }

    public IEnumerable<Block> GetAll()
    {
        return _sowScheduleDB.Blocks;
    }

    public bool Insert(Block entity)
    {
        _sowScheduleDB.Blocks.Add(entity);
        _sowScheduleDB.SaveChanges();
        return true;
    }

    public bool Remove(int pId)
    {
        Block block = _sowScheduleDB.Blocks.Find(pId);
        _sowScheduleDB.Blocks.Remove(block);
        _sowScheduleDB.SaveChanges();
        return true;
    }

    public bool Update(Block entity)
    {
        Block block = _sowScheduleDB.Blocks.Find(entity.Id);
        if (block != null)
        {
            block.OrderLocationId = entity.OrderLocationId;
            block.BlockNumber = entity.BlockNumber;
            block.SeedTrayAmount = entity.SeedTrayAmount;
            block.NumberWithinTheBlock = entity.NumberWithinTheBlock;
            _sowScheduleDB.SaveChanges();
            return true;
        }
        return false;
    }
}
