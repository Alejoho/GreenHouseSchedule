using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories
{
    public class TypeOfOrganizationRepository : GenericRepository, ITypeOfOrganizationRepository
    {
        public TypeOfOrganizationRepository(SowScheduleContext dbContex) : base(dbContex)
        {
        }

        public TypeOfOrganizationRepository()
        {
        }

        public IEnumerable<TypesOfOrganization> GetAll()
        {
            return _sowScheduleDB.TypesOfOrganizations;
        }
    }
}
