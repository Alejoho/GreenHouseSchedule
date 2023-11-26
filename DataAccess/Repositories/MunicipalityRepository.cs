using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories;

public class MunicipalityRepository : GenericRepository, IMunicipalityRepository
{
    public MunicipalityRepository()
    {
    }

    public MunicipalityRepository(SowScheduleContext dbContex) : base(dbContex)
    {
    }

    public IEnumerable<Municipality> GetAll()
    {
        return _sowScheduleDB.Municipalities;
    }

    public bool Insert(Municipality entity)
    {
        _sowScheduleDB.Municipalities.Add(entity);
        _sowScheduleDB.SaveChanges();

        return true;
    }

    public bool Remove(int pId)
    {
        Municipality municipality = _sowScheduleDB.Municipalities.Find((short)pId);
        _sowScheduleDB.Municipalities.Remove(municipality);
        _sowScheduleDB.SaveChanges();

        return true;
    }

    public bool Update(Municipality entity)
    {
        Municipality municipality = _sowScheduleDB.Municipalities.Find(entity.Id);
        if (municipality != null)
        {
            municipality.Name = entity.Name;
            municipality.ProvinceId = entity.ProvinceId;
            _sowScheduleDB.SaveChanges();
            return true;
        }

        return false;
    }
}
