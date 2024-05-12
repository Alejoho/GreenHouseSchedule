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
