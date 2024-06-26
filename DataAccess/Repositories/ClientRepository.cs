﻿using DataAccess.Context;
using DataAccess.Contracts;
using SupportLayer.Models;

namespace DataAccess.Repositories
{
    public class ClientRepository : GenericRepository, IClientRepository
    {
        public ClientRepository(SowScheduleContext dbContex) : base(dbContex)
        {
        }

        public ClientRepository()
        {
        }

        public IEnumerable<Client> GetAll()
        {
            var algo = _sowScheduleDB.Clients;
            return algo;
            //return _sowScheduleDB.Clients;            
        }

        public bool Insert(Client entity)
        {
            _sowScheduleDB.Clients.Add(entity);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Remove(int pId)
        {
            Client client = _sowScheduleDB.Clients.Find((short)pId);
            _sowScheduleDB.Clients.Remove(client);
            _sowScheduleDB.SaveChanges();
            return true;
        }

        public bool Update(Client entity)
        {
            Client client = _sowScheduleDB.Clients.Find(entity.Id);
            if (client != null)
            {
                client.Name = entity.Name;
                client.NickName = entity.NickName;
                client.PhoneNumber = entity.PhoneNumber;
                client.OtherNumber = entity.OtherNumber;
                client.OrganizationId = entity.OrganizationId;
                _sowScheduleDB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
