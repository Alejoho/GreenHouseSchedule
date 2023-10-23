using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Context;

namespace DataAccess.Repositories
{
    public abstract class GenericRepository
    {
        protected readonly SowScheduleContext _sowScheduleDB;

        public GenericRepository(SowScheduleContext dbContex)
        {
            _sowScheduleDB = dbContex;
        }

        public GenericRepository()
        {
            _sowScheduleDB = new SowScheduleContext();
        }
    }
}
