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
        //LATER - maybe exchange the order of ordenation.
        IEnumerable<OrderLocation> output = _repository.GetByASowDateOn(date)
            .OrderBy(x => x.Id)
            .ThenBy(x => x.SowDate);

        return output;
    }
}
