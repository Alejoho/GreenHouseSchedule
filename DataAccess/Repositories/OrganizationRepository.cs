using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using SupportLayer.Models;
using DataAccess.Context;

namespace DataAccess.Repositories
{
    public class OrganizationRepository : GenericRepository, IOrganizationRepository
    {
        public OrganizationRepository(SowScheduleDBEntities dbContex) : base(dbContex)
        {
        }

        public IEnumerable<Organization> GetAll()
        {
            return _sowScheduleDB.Organizations;
        }

        public bool Insert(Organization entity)
        {
            try
            {
                _sowScheduleDB.Organizations.Add(entity);
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
                Organization organization = _sowScheduleDB.Organizations.Find(pId);
                _sowScheduleDB.Organizations.Remove(organization);
                _sowScheduleDB.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(Organization entity)
        {
            Organization organization = _sowScheduleDB.Organizations.Find(entity.Id);
            if (organization != null)
            {
                organization.Name = entity.Name;
                organization.MunicipalitiesId = entity.MunicipalitiesId;
                organization.TypeOfOrganizationId = entity.TypeOfOrganizationId;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
