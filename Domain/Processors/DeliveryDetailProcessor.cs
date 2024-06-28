using DataAccess.Contracts;
using DataAccess.Repositories;
using log4net;
using SupportLayer;
using SupportLayer.Models;

namespace Domain.Processors;

public class DeliveryDetailProcessor : IDeliveryDetailProcessor
{
    private ILog _log;
    private IDeliveryDetailRepository _repository;

    public DeliveryDetailProcessor()
    {
        _repository = new DeliveryDetailRepository();
        _log = LogHelper.GetLogger();
    }

    public DeliveryDetailProcessor(ILog log, IDeliveryDetailRepository repository)
    {
        _repository = repository;
        _log = log;
    }

    public IEnumerable<DeliveryDetail> GetDeliveryDetailFromADateOn(DateOnly date)
    {
        IEnumerable<DeliveryDetail> output = _repository.GetByADeliveryDateOn(date)
            .OrderBy(x => x.DeliveryDate);

        return output;
    }

    public void SaveNewDeliveryDetail(Block block, DateOnly date, short deliveredSeedTrays)
    {
        ValidateDeliveryChanges(block, date, deliveredSeedTrays);

        DeliveryDetail deliveryDetail = new DeliveryDetail()
        {
            BlockId = block.Id,
            DeliveryDate = date,
            SeedTrayAmountDelivered = deliveredSeedTrays
        };

        _repository.Insert(deliveryDetail);

        block.DeliveryDetails.Add(deliveryDetail);

        _log.Info("Added a DeliveryDetail to the DB");
    }

    private void ValidateDeliveryChanges(Block block, DateOnly date, short deliveredSeedTrays)
    {
        if (date > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ArgumentException("La fecha debe ser igual o anterior que el dia presente", "date");
        }

        int seedTraysAlreadyDelivered = block.DeliveryDetails.Sum(x => x.SeedTrayAmountDelivered);
        int seedTraysToBeDelivered = block.SeedTrayAmount - seedTraysAlreadyDelivered;

        if (deliveredSeedTrays <= 0 || deliveredSeedTrays > seedTraysToBeDelivered)
        {
            throw new ArgumentException("La cantidad de bandejas entregadas debe estar entre 0 " +
                "y la cantidad de bandejas del bloque", "deliveredSeedTrays");
        }
    }
}
