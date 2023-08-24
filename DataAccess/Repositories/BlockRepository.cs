using DataAccess.Contracts;
using System;
using System.Collections.Generic;

namespace DataAccess.Repositories
{
    public class BlockRepository : GenericRepository, IBlockRepository
    {
        public BlockRepository(SowScheduleDBEntities dbContex) : base(dbContex)
        {
        }

        public IEnumerable<Block> GetAll()
        {
            return _sowScheduleDB.Blocks;
        }

        public bool Insert(Block entity)
        {
            try
            {
                _sowScheduleDB.Blocks.Add(entity);
                _sowScheduleDB.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Remove(int pId)
        {
            try
            {
                Block block = _sowScheduleDB.Blocks.Find(pId);
                _sowScheduleDB.Blocks.Remove(block);
                _sowScheduleDB.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public bool Update(Block entity)
        {
            Block block = _sowScheduleDB.Blocks.Find(entity.ID);
            if (block != null)
            {
                block.ID = entity.ID;
                block.OrderLocationId = entity.OrderLocationId;
                block.BlockNumber = entity.BlockNumber;
                block.SeedTrayAmount = entity.SeedTrayAmount;
                block.NumberWithinThBlock = entity.NumberWithinThBlock;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}