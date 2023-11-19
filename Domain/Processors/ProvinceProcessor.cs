using DataAccess.Contracts;
using DataAccess.Repositories;
using SupportLayer.Models;

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
