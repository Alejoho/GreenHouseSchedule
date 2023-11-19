using SupportLayer.Models;

namespace DataAccess.Contracts
{
    public interface ITypeOfOrganizationRepository
    {
        IEnumerable<TypesOfOrganization> GetAll();
    }
}
