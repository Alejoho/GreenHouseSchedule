using SupportLayer.Models;

namespace DataAccess.Contracts
{
    public interface IProvinceRepository
    {
        IEnumerable<Province> GetAll();
    }
}
