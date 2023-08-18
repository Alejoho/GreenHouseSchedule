using DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ClientRepository : GenericRepository, IClientRepository
    {
        public ClientRepository(SowScheduleDBEntities dbContex) : base(dbContex)
        {
        }

        public ClientRepository() 
        { 
        }

        public IEnumerable<Client> GetAll()
        {
            return _sowScheduleDB.Clients;            
        }

        public bool Insert(Client entity)
        {
            try
            {
                _sowScheduleDB.Clients.Add(entity);
                _sowScheduleDB.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Remove(int pId)
        {
            try
            {
                Client client = _sowScheduleDB.Clients.Find(pId);
                _sowScheduleDB.Clients.Remove(client);
                _sowScheduleDB.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Client entity)
        {
            Client client = _sowScheduleDB.Clients.Find(entity.ID);
            if (client != null)
            {   
                client.ID=entity.ID;
                client.Name=entity.Name;
                client.NickName=entity.NickName;
                client.PhoneNumber=entity.PhoneNumber;
                client.OtherNumber=entity.OtherNumber;
                client.OrganizationId=entity.OrganizationId;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
