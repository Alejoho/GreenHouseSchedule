using DataAccess.Contracts;
using DataAccess.Repositories;
using SupportLayer.Models;

namespace Domain.Processors
{
    public class TypeOfOrganizationProcessor
    {
        ITypeOfOrganizationRepository _repository;

        public TypeOfOrganizationProcessor()
        {
            _repository = new TypeOfOrganizationRepository();
        }

        public IEnumerable<TypesOfOrganization> GetAllTypesOfOrganizations()
        {
            return _repository.GetAll().OrderBy(x => x.Name);
        }
    }
}
