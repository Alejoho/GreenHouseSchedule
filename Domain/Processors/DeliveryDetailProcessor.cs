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

    public void SaveNewDeliveryDetails(Block block, DateOnly date, short deliveredSeedTrays)
    {
        ValidateDeliveryChanges(block, date, deliveredSeedTrays);

        DeliveryDetail deliveryDetail = new DeliveryDetail()
        {
            BlockId = block.Id,
            DeliveryDate = date,
            SeedTrayAmountDelivered = deliveredSeedTrays
        };

        _repository.Insert(deliveryDetail);

        block.SeedTrayAmount -= deliveredSeedTrays;
    }

    private void ValidateDeliveryChanges(Block block, DateOnly date, short deliveredSeedTrays)
    {
        if (date > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ArgumentException("La fecha debe ser igual o anterior que el dia presente", "date");
        }

        if (deliveredSeedTrays <= 0 || deliveredSeedTrays > block.SeedTrayAmount)
        {
            throw new ArgumentException("La cantidad de bandejas entregadas debe estar entre 0 " +
                "y la cantidad de bandejas del bloque", "deliveredSeedTrays");
        }
    }
}
