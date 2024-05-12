using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories;

public class OrganizationRepository : GenericRepository, IOrganizationRepository
{
    public OrganizationRepository(SowScheduleContext dbContex) : base(dbContex)
    {
    }

    public OrganizationRepository()
    {
    }

    public IEnumerable<Organization> GetAll()
    {
        return _sowScheduleDB.Organizations;
    }

    //LATER - Look up how can I use something similar to the "like" of SQL in EF Core 7
    public IEnumerable<Organization> GetSome(string filter)
    {
        return _sowScheduleDB.Organizations;
    }

    public bool Insert(Organization entity)
    {
        _sowScheduleDB.Organizations.Add(entity);
        _sowScheduleDB.SaveChanges();

        return true;
    }

    public bool Remove(int pId)
    {
        Organization organization = _sowScheduleDB.Organizations.Find((short)pId);
        _sowScheduleDB.Organizations.Remove(organization);
        _sowScheduleDB.SaveChanges();

        return true;
    }

    public bool Update(Organization entity)
    {
        Organization organization = _sowScheduleDB.Organizations.Find(entity.Id);
        if (organization != null)
        {
            organization.Name = entity.Name;
            organization.MunicipalityId = entity.MunicipalityId;
            organization.TypeOfOrganizationId = entity.TypeOfOrganizationId;
            _sowScheduleDB.SaveChanges();
            return true;
        }

        return false;
    }
}
