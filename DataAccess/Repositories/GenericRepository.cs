using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public abstract class GenericRepository
    {
        protected SowScheduleDBEntities _sowScheduleDB;

        public GenericRepository()
        {
            _sowScheduleDB = new SowScheduleDBEntities();
        }
    }
}
