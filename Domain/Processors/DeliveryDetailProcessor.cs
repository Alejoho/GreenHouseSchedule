using DataAccess.Repositories;
using SupportLayer.Models;

namespace Domain.Processors;

public class DeliveryDetailProcessor : IDeliveryDetailProcessor
{
    private DeliveryDetailRepository _repository;

    public DeliveryDetailProcessor()
    {
        _repository = new DeliveryDetailRepository();
    }

    public IEnumerable<DeliveryDetail> GetDeliveryDetailFromADateOn(DateOnly date)
    {
        IEnumerable<DeliveryDetail> output = _repository.GetByADeliveryDateOn(date)
            .OrderBy(x => x.DeliveryDate);

        return output;
    }
}
