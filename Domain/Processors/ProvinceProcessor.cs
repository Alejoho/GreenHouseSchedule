using DataAccess.Contracts;
using DataAccess.Repositories;
using SupportLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Processors
{
    public class ProvinceProcessor
    {
        private IProvinceRepository _repository;

        public ProvinceProcessor()
        {
            _repository = new ProvinceRepository();
        }

        public IEnumerable<Province> GetAllProvinces()
        {
            return _repository.GetAll().OrderBy(x => x.Name);
        }
    }
}
