using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportLayer.Models;
using DataAccess.Context;

namespace DataAccess.Repositories
{
    public class SpeciesRepository:GenericRepository,ISpeciesRepository
    {
        public SpeciesRepository(SowScheduleContext dbContex) :base(dbContex)
        { 
        }

        public SpeciesRepository()
        {           
        }

        public IEnumerable<Species> GetAll()
        {
            return _sowScheduleDB.Species;
        }

        public bool Insert(Species entity)
        {
                _sowScheduleDB.Species.Add(entity);
                _sowScheduleDB.SaveChanges();
                return true;
        }

        public bool Remove(int pId)
        {
                Species species = _sowScheduleDB.Species.Find((byte)pId);
                _sowScheduleDB.Species.Remove(species);
                _sowScheduleDB.SaveChanges();
                return true;
        }

        public bool Update(Species entity)
        {
            Species species = _sowScheduleDB.Species.Find(entity.Id);
            if (species != null)
            {
                species.Name = entity.Name;
                species.ProductionDays = entity.ProductionDays;
                species.WeightOf1000Seeds = entity.WeightOf1000Seeds;
                species.AmountOfSeedsPerHectare= entity.AmountOfSeedsPerHectare;
                species.WeightOfSeedsPerHectare=entity.WeightOfSeedsPerHectare;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
