using DataAccess.Repositories;
using SupportLayer.Models;

namespace Domain.Processors;

public class OrderLocationProcessor : IOrderLocationProcessor
{
    private OrderLocationRepository _repository;

    public OrderLocationProcessor()
    {
        _repository = new OrderLocationRepository();
    }

    public IEnumerable<OrderLocation> GetOrderLocationsFromADateOn(DateOnly date)
    {
        IEnumerable<OrderLocation> output = _repository.GetByASowDateOn(date)
            .OrderBy(x => x.SowDate)
            .ThenBy(x => x.Id);

        return output;
    }
}
